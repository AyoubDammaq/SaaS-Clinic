
import { useState } from 'react';
import { format } from 'date-fns';
import { 
  Tabs, 
  TabsContent, 
  TabsList, 
  TabsTrigger 
} from '@/components/ui/tabs';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Textarea } from '@/components/ui/textarea';
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Document } from '@/types/patient';
import { FileText, Trash2, Plus, Eye } from 'lucide-react';
import { CreateMedicalRecordModal } from './CreateMedicalRecordForm';

// Define local interfaces for this component
export interface MedicalRecord {
  id: string;
  allergies: string;
  chronicDiseases: string;
  currentMedications: string;
  familyHistory: string;
  personalHistory: string;
  bloodType: string;
  documents: MedicalRecordDocument[];
  creationDate: string;
  patientId: string;
}

export interface MedicalRecordDocument {
  id: string;
  name: string;
  type: string;
  url: string;
  uploadDate: string;
}

export interface Patient {
  id: string;
  name: string;
  email: string;
  phone: string;
  dateOfBirth: string;
  gender: string;
  address?: string;
  lastVisit?: string;
}

interface MedicalRecordViewProps {
  patient: Patient;
  medicalRecord: MedicalRecord | null;
  isLoading: boolean;
  isSubmitting: boolean;
  updateMedicalRecord: (data: Partial<Omit<MedicalRecord, "id" | "patientId">>) => Promise<void>;
  addDocument: (document: Omit<MedicalRecordDocument, "id">) => Promise<void>;
  deleteDocument: (documentId: string) => Promise<void>;
  onCreateMedicalRecord: (data: {
    allergies: string;
    chronicDiseases: string;
    currentMedications: string;
    bloodType: string;
    personalHistory: string;
    familyHistory: string;
  }) => Promise<void>;
  isCreating: boolean;
}

export function MedicalRecordView({
  patient,
  medicalRecord,
  isLoading,
  isSubmitting,
  updateMedicalRecord,
  addDocument,
  deleteDocument,
  onCreateMedicalRecord,
  isCreating
}: MedicalRecordViewProps) {
  const [isEditing, setIsEditing] = useState(false);
  const [formData, setFormData] = useState<Partial<MedicalRecord>>({});
  const [newDocument, setNewDocument] = useState({ name: '', type: 'PDF', url: '' });
  const [showAddDocument, setShowAddDocument] = useState(false);
  const [isFormOpen, setIsFormOpen] = useState(false);

  // Handle form input changes
  const handleInputChange = (field: keyof MedicalRecord, value: string) => {
    setFormData(prev => ({ ...prev, [field]: value }));
  };

  // Handle document input changes
  const handleDocumentInputChange = (field: keyof typeof newDocument, value: string) => {
    setNewDocument(prev => ({ ...prev, [field]: value }));
  };

  // Start editing
  const handleEdit = () => {
    if (medicalRecord) {
      setFormData({
        allergies: medicalRecord.allergies,
        chronicDiseases: medicalRecord.chronicDiseases,
        currentMedications: medicalRecord.currentMedications,
        familyHistory: medicalRecord.familyHistory,
        personalHistory: medicalRecord.personalHistory,
        bloodType: medicalRecord.bloodType,
      });
      setIsEditing(true);
    }
  };

  // Save changes
  const handleSave = async () => {
    await updateMedicalRecord(formData);
    setIsEditing(false);
  };

  // Add a new document
  const handleAddDocument = async () => {
    if (newDocument.name && newDocument.type) {
      await addDocument({
        ...newDocument,
        uploadDate: new Date().toISOString().split('T')[0]
      });
      setNewDocument({ name: '', type: 'PDF', url: '' });
      setShowAddDocument(false);
    }
  };

  if (isLoading) {
    return (
      <Card>
        <CardContent className="pt-6">
          <div className="flex justify-center items-center h-40">
            <p className="text-muted-foreground">Loading medical record...</p>
          </div>
        </CardContent>
      </Card>
    );
  }

  if (!medicalRecord) {
    return (
      <Card>
        <CardContent className="pt-6">
          <div className="flex justify-center items-center flex-col h-40">
            <p className="text-muted-foreground mb-4">No medical record found for this patient.</p>
            <Button variant="default" onClick={() => setIsFormOpen(true)}>
                Créer un dossier médical
              </Button>
              {isFormOpen && (
                <CreateMedicalRecordModal
                  isOpen={isFormOpen}
                  onClose={() => setIsFormOpen(false)}
                  onSubmit={onCreateMedicalRecord}
                  isSubmitting={isCreating}
                />
              )}
          </div>
        </CardContent>
      </Card>
    );
  }

  return (
    <div className="space-y-6">
      <Card>
        <CardHeader>
          <div className="flex justify-between items-center">
            <div>
              <CardTitle>Medical Record</CardTitle>
              <CardDescription>
                Patient: {patient.name} | Created: {medicalRecord.creationDate}
              </CardDescription>
            </div>
            {!isEditing ? (
              <Button onClick={handleEdit} variant="outline">Edit Medical Record</Button>
            ) : (
              <div className="flex gap-2">
                <Button onClick={() => setIsEditing(false)} variant="outline">Cancel</Button>
                <Button onClick={handleSave} disabled={isSubmitting}>
                  {isSubmitting ? 'Saving...' : 'Save Changes'}
                </Button>
              </div>
            )}
          </div>
        </CardHeader>
        <CardContent>
          <Tabs defaultValue="general" className="w-full">
            <TabsList className="grid grid-cols-2 md:grid-cols-4 mb-4">
              <TabsTrigger value="general">General</TabsTrigger>
              <TabsTrigger value="history">Medical History</TabsTrigger>
              <TabsTrigger value="medications">Medications</TabsTrigger>
              <TabsTrigger value="documents">Documents</TabsTrigger>
            </TabsList>
            
            <TabsContent value="general">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div>
                  <h3 className="text-sm font-medium mb-2">Allergies</h3>
                  {isEditing ? (
                    <Textarea
                      value={formData.allergies || ''}
                      onChange={(e) => handleInputChange('allergies', e.target.value)}
                      placeholder="List patient allergies"
                      className="min-h-[100px]"
                    />
                  ) : (
                    <p className="text-sm text-muted-foreground whitespace-pre-wrap">
                      {medicalRecord.allergies || 'None recorded'}
                    </p>
                  )}
                </div>
                <div>
                  <h3 className="text-sm font-medium mb-2">Blood Type</h3>
                  {isEditing ? (
                    <Select 
                      value={formData.bloodType || ''}
                      onValueChange={(value) => handleInputChange('bloodType', value)}
                    >
                      <SelectTrigger>
                        <SelectValue placeholder="Select blood type" />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="A+">A+</SelectItem>
                        <SelectItem value="A-">A-</SelectItem>
                        <SelectItem value="B+">B+</SelectItem>
                        <SelectItem value="B-">B-</SelectItem>
                        <SelectItem value="AB+">AB+</SelectItem>
                        <SelectItem value="AB-">AB-</SelectItem>
                        <SelectItem value="O+">O+</SelectItem>
                        <SelectItem value="O-">O-</SelectItem>
                        <SelectItem value="Unknown">Unknown</SelectItem>
                      </SelectContent>
                    </Select>
                  ) : (
                    <p className="text-sm text-muted-foreground">
                      {medicalRecord.bloodType || 'Unknown'}
                    </p>
                  )}
                </div>
                <div>
                  <h3 className="text-sm font-medium mb-2">Chronic Diseases</h3>
                  {isEditing ? (
                    <Textarea
                      value={formData.chronicDiseases || ''}
                      onChange={(e) => handleInputChange('chronicDiseases', e.target.value)}
                      placeholder="List chronic conditions"
                      className="min-h-[100px]"
                    />
                  ) : (
                    <p className="text-sm text-muted-foreground whitespace-pre-wrap">
                      {medicalRecord.chronicDiseases || 'None recorded'}
                    </p>
                  )}
                </div>
              </div>
            </TabsContent>
            
            <TabsContent value="history">
              <div className="grid grid-cols-1 gap-6">
                <div>
                  <h3 className="text-sm font-medium mb-2">Personal Medical History</h3>
                  {isEditing ? (
                    <Textarea
                      value={formData.personalHistory || ''}
                      onChange={(e) => handleInputChange('personalHistory', e.target.value)}
                      placeholder="Details of past medical procedures, hospitalizations, etc."
                      className="min-h-[150px]"
                    />
                  ) : (
                    <p className="text-sm text-muted-foreground whitespace-pre-wrap">
                      {medicalRecord.personalHistory || 'No history recorded'}
                    </p>
                  )}
                </div>
                
                <div>
                  <h3 className="text-sm font-medium mb-2">Family Medical History</h3>
                  {isEditing ? (
                    <Textarea
                      value={formData.familyHistory || ''}
                      onChange={(e) => handleInputChange('familyHistory', e.target.value)}
                      placeholder="Details of relevant family medical history"
                      className="min-h-[150px]"
                    />
                  ) : (
                    <p className="text-sm text-muted-foreground whitespace-pre-wrap">
                      {medicalRecord.familyHistory || 'No family history recorded'}
                    </p>
                  )}
                </div>
              </div>
            </TabsContent>
            
            <TabsContent value="medications">
              <div>
                <h3 className="text-sm font-medium mb-2">Current Medications</h3>
                {isEditing ? (
                  <Textarea
                    value={formData.currentMedications || ''}
                    onChange={(e) => handleInputChange('currentMedications', e.target.value)}
                    placeholder="List all current medications with dosage"
                    className="min-h-[200px]"
                  />
                ) : (
                  <p className="text-sm text-muted-foreground whitespace-pre-wrap">
                    {medicalRecord.currentMedications || 'No medications recorded'}
                  </p>
                )}
              </div>
            </TabsContent>
            
            <TabsContent value="documents">
              <div className="space-y-4">
                <div className="flex justify-between items-center">
                  <h3 className="text-sm font-medium">Medical Documents</h3>
                  <Button 
                    variant="outline" 
                    size="sm" 
                    onClick={() => setShowAddDocument(!showAddDocument)}
                  >
                    <Plus className="mr-1" size={16} /> Add Document
                  </Button>
                </div>
                
                {showAddDocument && (
                  <Card className="border border-dashed p-4">
                    <div className="space-y-3">
                      <Input 
                        placeholder="Document Name" 
                        value={newDocument.name}
                        onChange={(e) => handleDocumentInputChange('name', e.target.value)}
                      />
                      <Select 
                        value={newDocument.type}
                        onValueChange={(value) => handleDocumentInputChange('type', value)}
                      >
                        <SelectTrigger>
                          <SelectValue placeholder="Document Type" />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectItem value="PDF">PDF</SelectItem>
                          <SelectItem value="Image">Image</SelectItem>
                          <SelectItem value="Lab Report">Lab Report</SelectItem>
                          <SelectItem value="Prescription">Prescription</SelectItem>
                          <SelectItem value="Other">Other</SelectItem>
                        </SelectContent>
                      </Select>
                      <Input 
                        placeholder="Document URL (optional)" 
                        value={newDocument.url}
                        onChange={(e) => handleDocumentInputChange('url', e.target.value)}
                      />
                      <div className="flex justify-end gap-2">
                        <Button 
                          variant="outline" 
                          size="sm" 
                          onClick={() => setShowAddDocument(false)}
                        >
                          Cancel
                        </Button>
                        <Button 
                          size="sm" 
                          onClick={handleAddDocument}
                          disabled={!newDocument.name}
                        >
                          Add Document
                        </Button>
                      </div>
                    </div>
                  </Card>
                )}
                
                {medicalRecord.documents.length === 0 ? (
                  <p className="text-sm text-muted-foreground py-4">No documents available</p>
                ) : (
                  <div className="border rounded-md overflow-hidden">
                    <table className="w-full">
                      <thead>
                        <tr className="bg-muted/50">
                          <th className="text-left p-2 text-sm font-medium">Name</th>
                          <th className="text-left p-2 text-sm font-medium">Type</th>
                          <th className="text-left p-2 text-sm font-medium">Upload Date</th>
                          <th className="text-right p-2 text-sm font-medium">Actions</th>
                        </tr>
                      </thead>
                      <tbody>
                        {medicalRecord.documents.map((doc) => (
                          <tr key={doc.id} className="border-t">
                            <td className="p-2 text-sm">
                              <div className="flex items-center">
                                <FileText size={16} className="mr-2 text-muted-foreground" />
                                {doc.name}
                              </div>
                            </td>
                            <td className="p-2 text-sm">{doc.type}</td>
                            <td className="p-2 text-sm">{doc.uploadDate}</td>
                            <td className="p-2 text-right">
                              <div className="flex justify-end gap-1">
                                {doc.url && (
                                  <Button variant="ghost" size="sm">
                                    <Eye size={16} />
                                  </Button>
                                )}
                                <Button 
                                  variant="ghost" 
                                  size="sm"
                                  onClick={() => deleteDocument(doc.id)}
                                >
                                  <Trash2 size={16} className="text-red-500" />
                                </Button>
                              </div>
                            </td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                )}
              </div>
            </TabsContent>
          </Tabs>
        </CardContent>
      </Card>
    </div>
  );
}
