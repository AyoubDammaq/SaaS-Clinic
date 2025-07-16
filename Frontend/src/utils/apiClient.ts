import { DEFAULT_HEADERS, HTTP_METHODS, API_ENDPOINTS } from "@/config/api";
import { RefreshTokenRequestDto, TokenResponseDto } from "@/types/auth";
import { toast } from "sonner";

interface RequestOptions {
  method: HTTP_METHODS;
  headers?: Record<string, string>;
  body?: unknown;
  withAuth?: boolean;
  retryCount?: number;
  maxRetries?: number;
  retryDelay?: number;
}

/**
 * API client for making HTTP requests
 * @param url The URL to make the request to
 * @param options The request options
 * @returns The response data
 */
export async function apiClient<T = unknown>(
  url: string,
  options: RequestOptions
): Promise<T> {
  try {
    const {
      method,
      body,
      withAuth = true,
      retryCount = 0,
      maxRetries = 3,
      retryDelay = 2000, // Default retry delay in ms
    } = options;

    // Merge default headers with provided headers
    const headers = {
      ...DEFAULT_HEADERS,
      ...options.headers,
    };

    // Add authorization header if required
    if (withAuth) {
      const token = localStorage.getItem("accessToken");
      if (token) {
        headers["Authorization"] = `Bearer ${token}`;
      }
    }

    const requestOptions: RequestInit = {
      method,
      headers,
      credentials: "include", // Include cookies in the request
    };

    // Add body if provided (and not GET request)
    if (body && method !== HTTP_METHODS.GET) {
      if (body instanceof FormData) {
        requestOptions.body = body;
        // Important : ne pas mettre Content-Type, le navigateur le gère
        delete headers["Content-Type"];
      } else {
        requestOptions.body = JSON.stringify(body);
      }
    }

    console.log(`API Request to ${url}`, {
      method,
      body: method !== HTTP_METHODS.GET ? body : undefined,
      headers: {
        ...headers,
        Authorization: withAuth ? "Bearer ***" : undefined,
      },
    });

    const response = await fetch(url, requestOptions);

    // Handle rate limiting (429 Too Many Requests)
    if (response.status === 429) {
      // Calculate retry delay - use header value if available, otherwise use exponential backoff
      let waitTime = retryDelay;

      if (response.headers.get("Retry-After")) {
        // Retry-After header can be in seconds or a date
        if (!isNaN(Number(response.headers.get("Retry-After")))) {
          waitTime = Number(response.headers.get("Retry-After")) * 1000; // Convert seconds to ms
        } else {
          const retryDate = new Date(response.headers.get("Retry-After"));
          if (!isNaN(retryDate.getTime())) {
            waitTime = retryDate.getTime() - Date.now();
          }
        }
      } else {
        // Exponential backoff with jitter for distributed systems
        const exponentialDelay = Math.min(
          30000,
          retryDelay * Math.pow(2, retryCount)
        ); // Max 30 seconds
        waitTime = exponentialDelay * (0.8 + Math.random() * 0.4); // Add jitter (±20%)
      }

      if (retryCount < maxRetries) {
        console.warn(
          `Rate limited. Retrying after ${
            waitTime / 1000
          } seconds. Retry attempt ${retryCount + 1}/${maxRetries}`
        );
        toast.warning(
          `API rate limit reached. Retrying in ${Math.ceil(
            waitTime / 1000
          )} seconds...`
        );

        // Wait for the calculated time
        await new Promise((resolve) => setTimeout(resolve, waitTime));

        // Retry the request
        return apiClient<T>(url, {
          ...options,
          retryCount: retryCount + 1,
          retryDelay: waitTime, // Pass the current delay for exponential backoff
        });
      } else {
        throw new Error("API rate limit exceeded. Please try again later.");
      }
    }

    // For debugging purposes
    console.log(`API Response from ${url}`, {
      status: response.status,
      statusText: response.statusText,
    });

    // Handle HTTP errors
    if (!response.ok) {
      // Try to parse error data
      let errorMessage: string;
      try {
        const errorData = await response.json();
        console.log("Error data:", errorData);
        errorMessage =
          errorData.message ||
          errorData.title ||
          `Error: ${response.status} ${response.statusText}`;
      } catch (parseError) {
        errorMessage = `Error: ${response.status} ${response.statusText}`;
      }

      // Handle authentication errors
      if (response.status === 401) {
        // Try to refresh the token
        const refreshToken = localStorage.getItem("refreshToken");

        if (refreshToken) {
          try {
            const refreshResult = await refreshTokens(refreshToken);
            if (refreshResult) {
              // Retry the original request with new token
              return apiClient<T>(url, {
                ...options,
                retryCount: 0, // Reset retry count for the new request
              });
            }
          } catch (refreshError) {
            // Token refresh failed, clear auth data and redirect to login
            localStorage.removeItem("accessToken");
            localStorage.removeItem("refreshToken");
            localStorage.removeItem("user");
            window.location.href = "/login";
            throw new Error("Authentication failed. Please log in again.");
          }
        } else {
          // No refresh token, clear auth data and redirect to login
          localStorage.removeItem("accessToken");
          localStorage.removeItem("refreshToken");
          localStorage.removeItem("user");
          window.location.href = "/login";
          throw new Error("Authentication failed. Please log in again.");
        }
      }

      throw new Error(errorMessage);
    }

    // Handle 204 No Content or empty body
    const contentLength = response.headers.get("content-length");
    const contentType = response.headers.get("content-type");

    if (
      response.status === 204 ||
      !contentLength ||
      parseInt(contentLength) === 0 ||
      !contentType ||
      !contentType.includes("application/json")
    ) {
      return {} as T; // or `null` if that's what you expect
    }

    // Parse JSON response safely
    try {
      const data = await response.json();
      console.log(`API Success data from ${url}:`, data);
      return data as T;
    } catch (parseError) {
      console.warn(`Could not parse JSON from ${url}:`, parseError);
      return {} as T;
    }
  } catch (error) {
    // Log the error
    console.error("API request failed:", error);

    // Show error toast
    if (error instanceof Error) {
      toast.error(error.message);
    } else {
      toast.error("An unexpected error occurred");
    }

    throw error;
  }
}

/**
 * Function to refresh the access token
 * @param refreshToken The refresh token
 * @returns True if refresh was successful
 */
async function refreshTokens(refreshToken: string): Promise<boolean> {
  // Use the configured API endpoint
  const response = await fetch(API_ENDPOINTS.AUTH.REFRESH_TOKEN, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({ refreshToken } as RefreshTokenRequestDto),
  });

  if (!response.ok) {
    return false;
  }

  const tokenData = (await response.json()) as TokenResponseDto;

  // Store the new tokens
  localStorage.setItem("accessToken", tokenData.accessToken);
  localStorage.setItem("refreshToken", tokenData.refreshToken);

  // Update user information if provided
  if (tokenData.user) {
    localStorage.setItem("user", JSON.stringify(tokenData.user));
  }

  return true;
}

/**
 * Convenience methods for common HTTP requests
 */
export const api = {
  get: <T = unknown>(url: string, options?: Partial<RequestOptions>) =>
    apiClient<T>(url, { method: HTTP_METHODS.GET, ...options }),

  post: <T = unknown>(
    url: string,
    body?: unknown,
    options?: Partial<RequestOptions>
  ) => apiClient<T>(url, { method: HTTP_METHODS.POST, body, ...options }),

  put: <T = unknown>(
    url: string,
    body?: unknown,
    options?: Partial<RequestOptions>
  ) => apiClient<T>(url, { method: HTTP_METHODS.PUT, body, ...options }),

  delete: <T = unknown>(url: string, options?: Partial<RequestOptions>) =>
    apiClient<T>(url, { method: HTTP_METHODS.DELETE, ...options }),
};
