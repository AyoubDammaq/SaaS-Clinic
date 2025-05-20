
import { useState, useMemo, useCallback } from 'react';
import { Invoice, InvoiceStatus, PaymentDetails } from '@/types/billing';
import { useAuth } from '@/contexts/AuthContext';
import { toast } from 'sonner';

interface UseBillingOptions {
  initialInvoices?: Invoice[];
  itemsPerPage?: number;
  autoFetch?: boolean;
}

export function useBilling({ 
  initialInvoices = [], 
  itemsPerPage = 5,
  autoFetch = true
}: UseBillingOptions = {}) {
  const { user } = useAuth();
  const [invoices, setInvoices] = useState<Invoice[]>(initialInvoices);
  const [searchTerm, setSearchTerm] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const [sortField, setSortField] = useState<keyof Invoice>('date');
  const [sortDirection, setSortDirection] = useState<'asc' | 'desc'>('desc');
  const [selectedInvoice, setSelectedInvoice] = useState<Invoice | null>(null);
  const [isPaymentFormOpen, setIsPaymentFormOpen] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Fetch invoices - in a real app, this would call an API
  const fetchInvoices = useCallback(async () => {
    if (!autoFetch) return;
    
    setIsLoading(true);
    setError(null);
    
    try {
      // Simulate API call
      await new Promise(resolve => setTimeout(resolve, 800));
      
      // In a real app, you'd fetch from an API here
      // const response = await fetch('/api/invoices');
      // const data = await response.json();
      // setInvoices(data);
      
      setInvoices(initialInvoices);
    } catch (err) {
      console.error('Failed to fetch invoices:', err);
      setError('Failed to load invoices. Please try again.');
      toast.error('Failed to load invoices');
    } finally {
      setIsLoading(false);
    }
  }, [initialInvoices, autoFetch]);

  // Filter invoices based on search term and user role
  const filteredInvoices = useMemo(() => {
    let filtered = invoices;
    
    // Filter by search term
    if (searchTerm) {
      const searchLower = searchTerm.toLowerCase();
      filtered = filtered.filter(invoice => (
        invoice.id.toLowerCase().includes(searchLower) ||
        invoice.patient.toLowerCase().includes(searchLower) ||
        invoice.description.toLowerCase().includes(searchLower)
      ));
    }
    
    // Filter by user role (patients only see their own invoices)
    if (user?.role === 'Patient') {
      filtered = filtered.filter(invoice => 
        invoice.patient.toLowerCase().includes((user.name || '').toLowerCase())
      );
    }
    
    return filtered;
  }, [invoices, searchTerm, user]);

  // Sort invoices
  const sortedInvoices = useMemo(() => {
    return [...filteredInvoices].sort((a, b) => {
      let comparison = 0;
      
      // Handle different field types
      if (sortField === 'amount') {
        comparison = a.amount - b.amount;
      } else if (sortField === 'date' || sortField === 'dueDate') {
        // Sort dates
        const dateA = new Date(a[sortField]);
        const dateB = new Date(b[sortField]);
        comparison = dateA.getTime() - dateB.getTime();
      } else {
        comparison = String(a[sortField]).localeCompare(String(b[sortField]));
      }
      
      return sortDirection === 'desc' ? -comparison : comparison;
    });
  }, [filteredInvoices, sortField, sortDirection]);

  // Calculate pagination
  const totalPages = Math.ceil(sortedInvoices.length / itemsPerPage);
  const paginatedInvoices = useMemo(() => {
    const startIndex = (currentPage - 1) * itemsPerPage;
    return sortedInvoices.slice(startIndex, startIndex + itemsPerPage);
  }, [sortedInvoices, currentPage, itemsPerPage]);

  // Handle sorting changes
  const handleSort = useCallback((field: keyof Invoice) => {
    if (field === sortField) {
      setSortDirection(sortDirection === 'asc' ? 'desc' : 'asc');
    } else {
      setSortField(field);
      setSortDirection('asc');
    }
  }, [sortField, sortDirection]);

  // Reset pagination when filters change
  useEffect(() => {
    setCurrentPage(1);
  }, [searchTerm, sortField, sortDirection]);

  // Initial fetch
  useEffect(() => {
    fetchInvoices();
  }, [fetchInvoices]);

  // Handle payment form
  const handlePayNow = useCallback((invoice: Invoice) => {
    setSelectedInvoice(invoice);
    setIsPaymentFormOpen(true);
  }, []);

  // Handle payment submission
  const handlePaymentSubmit = useCallback((data: PaymentDetails) => {
    if (!selectedInvoice) return;
    
    try {
      // Update invoice status
      const updatedInvoices = invoices.map(invoice => 
        invoice.id === selectedInvoice.id ? { ...invoice, status: 'Paid' as InvoiceStatus } : invoice
      );
      
      setInvoices(updatedInvoices);
      closePaymentForm();
      
      // In a real app, you'd send this to your backend
      console.log('Payment processed:', { invoiceId: selectedInvoice.id, ...data });
      toast.success('Payment successfully processed');
    } catch (error) {
      console.error('Payment error:', error);
      toast.error('Payment processing failed. Please try again.');
    }
  }, [selectedInvoice, invoices]);

  const closePaymentForm = useCallback(() => {
    setIsPaymentFormOpen(false);
    setSelectedInvoice(null);
  }, []);

  // Handle invoice download
  const handleDownload = useCallback((invoice: Invoice) => {
    // In a real app, this would trigger a PDF download
    toast.info(`Downloading invoice ${invoice.id}...`);
    console.log('Downloading invoice:', invoice.id);
    
    // Simulate download
    setTimeout(() => {
      toast.success(`Invoice ${invoice.id} downloaded`);
    }, 1500);
  }, []);

  // Check user permissions
  const userPermissions = useMemo(() => {
    return {
      canPayInvoices: user?.role !== undefined,
      canViewAllInvoices: user?.role === 'SuperAdmin' || user?.role === 'ClinicAdmin',
      canGenerateReports: user?.role === 'SuperAdmin' || user?.role === 'ClinicAdmin'
    };
  }, [user?.role]);

  // Refresh data
  const refreshInvoices = useCallback(() => {
    fetchInvoices();
  }, [fetchInvoices]);

  return {
    invoices: paginatedInvoices,
    isLoading,
    error,
    filteredCount: filteredInvoices.length,
    totalCount: invoices.length,
    currentPage,
    totalPages,
    searchTerm,
    setSearchTerm,
    setCurrentPage,
    sortField,
    sortDirection,
    handleSort,
    selectedInvoice,
    isPaymentFormOpen,
    handlePayNow,
    handlePaymentSubmit,
    closePaymentForm,
    handleDownload,
    refreshInvoices,
    userPermissions
  };
}

import { useEffect } from 'react';
