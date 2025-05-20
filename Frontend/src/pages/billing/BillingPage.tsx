
import { useAuth } from '@/contexts/AuthContext';
import { Card, CardHeader, CardTitle, CardDescription, CardContent, CardFooter } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Calendar, Download, RefreshCcw } from 'lucide-react';
import { PaymentForm } from '@/components/billing/PaymentForm';
import { InvoiceTable } from '@/components/billing/InvoiceTable';
import { SearchBar } from '@/components/billing/SearchBar';
import { EmptyState } from '@/components/billing/EmptyState';
import { PaginationControls } from '@/components/ui/pagination-controls';
import { useBilling } from '@/hooks/useBilling';
import { Invoice, InvoiceStatus } from '@/types/billing';
import { LoadingSpinner } from '@/components/ui/loading-spinner';
import { DashboardLayout } from '@/components/layout/DashboardLayout';

// Sample data - in a real app, this would come from an API
const mockInvoices: Invoice[] = [
  {
    id: 'INV-001',
    patient: 'John Doe',
    date: '2025-04-15',
    amount: 150.00,
    status: 'Paid' as InvoiceStatus,
    description: 'General consultation',
    dueDate: '2025-04-30'
  },
  {
    id: 'INV-002',
    patient: 'Jane Smith',
    date: '2025-04-18',
    amount: 250.00,
    status: 'Pending' as InvoiceStatus,
    description: 'Lab tests and analysis',
    dueDate: '2025-05-02'
  },
  {
    id: 'INV-003',
    patient: 'Robert Brown',
    date: '2025-04-20',
    amount: 75.50,
    status: 'Overdue' as InvoiceStatus,
    description: 'Prescription refill',
    dueDate: '2025-04-27'
  },
  {
    id: 'INV-004',
    patient: 'Emily Davis',
    date: '2025-04-23',
    amount: 320.00,
    status: 'Pending' as InvoiceStatus,
    description: 'Specialist consultation',
    dueDate: '2025-05-07'
  },
  {
    id: 'INV-005',
    patient: 'Michael Wilson',
    date: '2025-04-25',
    amount: 95.75,
    status: 'Pending' as InvoiceStatus,
    description: 'Follow-up visit',
    dueDate: '2025-05-10'
  }
];

function BillingPage() {
  const { user } = useAuth();
  const {
    invoices,
    isLoading,
    error,
    filteredCount,
    totalCount,
    currentPage,
    totalPages,
    searchTerm,
    setSearchTerm,
    setCurrentPage,
    selectedInvoice,
    isPaymentFormOpen,
    handlePayNow,
    handlePaymentSubmit,
    closePaymentForm,
    handleDownload,
    refreshInvoices,
    userPermissions
  } = useBilling({ initialInvoices: mockInvoices });

  // Determine whether to show patient column based on user role
  const showPatientColumn = user?.role !== 'Patient';
  
  // Get page title and description based on user role
  const getPageContent = () => {
    if (user?.role === 'Patient') {
      return {
        title: "Billing",
        description: "View and manage your invoices"
      };
    } else {
      return {
        title: "Billing Management",
        description: "Manage invoices for patients"
      };
    }
  };

  const { title, description } = getPageContent();

  return (
    <div className="space-y-6 pb-8">
      <div className="flex flex-col gap-2">
        <h1 className="text-3xl font-bold tracking-tight">{title}</h1>
        <p className="text-muted-foreground" id="billing-description">
          {description}
        </p>
      </div>

      <Card className="card-hover">
        <CardHeader>
          <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
            <div>
              <CardTitle>Invoices</CardTitle>
              <CardDescription>
                {user?.role === 'Patient'
                  ? "Your recent invoices"
                  : "Recent invoices"}
              </CardDescription>
            </div>
            <div className="flex flex-col sm:flex-row items-center gap-2">
              <SearchBar 
                value={searchTerm} 
                onChange={setSearchTerm} 
                placeholder="Search invoices"
                className="w-full sm:w-auto"
              />
              <Button 
                variant="outline" 
                size="icon"
                onClick={refreshInvoices}
                aria-label="Refresh invoices"
                className="shrink-0"
              >
                <RefreshCcw className="h-4 w-4" />
              </Button>
            </div>
          </div>
        </CardHeader>
        <CardContent>
          {isLoading ? (
            <LoadingSpinner fullPage text="Loading invoices" />
          ) : error ? (
            <EmptyState
              title="Error loading invoices"
              description={error}
              action={{
                label: "Try again",
                onClick: refreshInvoices
              }}
            />
          ) : (
            <>
              <InvoiceTable 
                invoices={invoices} 
                showPatientColumn={showPatientColumn}
                onPayNow={handlePayNow}
                onDownload={handleDownload}
              />
              
              {invoices.length === 0 && searchTerm && (
                <EmptyState 
                  title="No invoices found" 
                  description="No invoices match your search"
                  action={{
                    label: "Clear search",
                    onClick: () => setSearchTerm('')
                  }}
                />
              )}
              
              {invoices.length === 0 && !searchTerm && (
                <EmptyState 
                  title="No invoices" 
                  description="You don't have any invoices yet"
                />
              )}
            </>
          )}
        </CardContent>
        <CardFooter className="border-t pt-4 flex flex-col sm:flex-row justify-between gap-4">
          <div className="text-sm text-muted-foreground">
            {isLoading ? (
              <span>&nbsp;</span>
            ) : (
              `Showing ${invoices.length} of ${filteredCount} invoices`
            )}
          </div>
          
          {totalPages > 1 && (
            <PaginationControls
              currentPage={currentPage}
              totalPages={totalPages}
              onPageChange={setCurrentPage}
            />
          )}
          
          <div className="flex items-center gap-2 ml-auto">
            {userPermissions.canGenerateReports && (
              <Button variant="outline" size="sm" className="btn-press">
                <Calendar className="mr-1 h-4 w-4" aria-hidden="true" />
                Payment History
              </Button>
            )}
          </div>
        </CardFooter>
      </Card>

      {/* Payment Form */}
      {selectedInvoice && (
        <PaymentForm
          isOpen={isPaymentFormOpen}
          onClose={closePaymentForm}
          onSubmit={handlePaymentSubmit}
          invoiceAmount={selectedInvoice.amount}
          invoiceId={selectedInvoice.id}
        />
      )}
    </div>
  );
}

export default BillingPage;
