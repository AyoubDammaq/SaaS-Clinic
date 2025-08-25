import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "@/hooks/useAuth";
import { User } from "@/types/auth";
import { UsersList } from "@/components/users/UsersList";
import { useUsers } from "@/hooks/useUsers";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
  DialogFooter,
} from "@/components/ui/dialog";
import { useTranslation } from "@/hooks/useTranslation";

function UsersPage() {
  const { t } = useTranslation("users");
  const { user } = useAuth();
  const navigate = useNavigate();
  const {
    users,
    filteredUsers,
    isLoading,
    permissions,
    searchTerm,
    setSearchTerm,
    handleAddUser,
    handleUpdateUser,
    handleDeleteUser,
  } = useUsers();

  const [isFormOpen, setIsFormOpen] = useState(false);
  const [selectedUser, setSelectedUser] = useState<User | null>(null);
  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
  const [userToDelete, setUserToDelete] = useState<string | null>(null);
  const [currentPage, setCurrentPage] = useState(1);

  // Pagination constants
  const ITEMS_PER_PAGE = 10;
  const totalPages = Math.ceil(filteredUsers.length / ITEMS_PER_PAGE);
  const startIndex = (currentPage - 1) * ITEMS_PER_PAGE;
  const paginatedUsers = filteredUsers.slice(
    startIndex,
    startIndex + ITEMS_PER_PAGE
  );

  // Reset currentPage when filteredUsers changes
  useEffect(() => {
    if (filteredUsers.length === 0) setCurrentPage(1);
    else if (currentPage > totalPages && totalPages > 0)
      setCurrentPage(totalPages);
  }, [filteredUsers, totalPages, currentPage]);

  // Handle form submission
  const handleFormSubmit = async (
    data: Omit<User, "id" | "dateCreation">
  ): Promise<User> => {
    try {
      if (selectedUser) {
        const payload = { ...data, id: selectedUser.id };
        const updatedUser = await handleUpdateUser(selectedUser.id, payload);
        setSelectedUser(null);
        setIsFormOpen(false);
        return updatedUser;
      } else {
        const newUser = await handleAddUser(data);
        setIsFormOpen(false);
        return newUser;
      }
    } catch (error) {
      console.error("Error submitting user data:", error);
      throw error;
    }
  };

  const handleEditUser = (user: User) => {
    setSelectedUser(user);
    setIsFormOpen(true);
  };

  const handleDeleteUserConfirm = async () => {
    if (userToDelete) {
      try {
        await handleDeleteUser(userToDelete);
        setIsDeleteDialogOpen(false);
        setUserToDelete(null);
      } catch (error) {
        console.error("Error deleting user:", error);
      }
    }
  };

  const handleDeleteUserRequest = (userId: string) => {
    setUserToDelete(userId);
    setIsDeleteDialogOpen(true);
  };

  return (
    <div className="space-y-6 pb-8">
      <div className="flex flex-col gap-2">
        <h1 className="text-3xl font-bold tracking-tight">{t("users")}</h1>
        <p className="text-muted-foreground">{t("manage_users_description")}</p>
      </div>

      <UsersList
        users={users}
        filteredUsers={paginatedUsers} // pagination gérée ici
        userRole={user?.role || "SuperAdmin"}
        searchTerm={searchTerm}
        setSearchTerm={setSearchTerm}
        isLoading={isLoading}
        permissions={permissions}
        onAddUser={() => {
          setSelectedUser(null);
          setIsFormOpen(true);
        }}
        onEditUser={handleEditUser}
        onDeleteUser={handleDeleteUserRequest}
      />

      {/* Pagination */}
      {totalPages > 1 && (
        <div className="flex justify-center items-center gap-4 mt-4">
          <Button
            size="sm"
            variant="outline"
            onClick={() => setCurrentPage((prev) => Math.max(prev - 1, 1))}
            disabled={currentPage === 1}
          >
            {t("previous")}
          </Button>
          <span className="text-sm text-muted-foreground">
            {t("page")} {currentPage} {t("of")} {totalPages}
          </span>
          <Button
            size="sm"
            variant="outline"
            onClick={() =>
              setCurrentPage((prev) => Math.min(prev + 1, totalPages))
            }
            disabled={currentPage === totalPages}
          >
            {t("next")}
          </Button>
        </div>
      )}

      {/* Confirm Delete Dialog */}
      <Dialog open={isDeleteDialogOpen} onOpenChange={setIsDeleteDialogOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>{t("confirmDelete")}</DialogTitle>
            <DialogDescription>
              {t("confirmDeleteDescription")}
            </DialogDescription>
          </DialogHeader>
          <DialogFooter>
            <Button
              variant="outline"
              onClick={() => setIsDeleteDialogOpen(false)}
            >
              {t("cancel")}
            </Button>
            <Button variant="destructive" onClick={handleDeleteUserConfirm}>
              {t("confirm")}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}

export default UsersPage;
