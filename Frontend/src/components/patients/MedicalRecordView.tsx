import { useState } from "react";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
  CardDescription,
} from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Textarea } from "@/components/ui/textarea";
import { Input } from "@/components/ui/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { FileText, Trash2, Plus, Eye } from "lucide-react";
import { CreateMedicalRecordModal } from "./CreateMedicalRecordForm";
import { UserRole } from "@/types/auth";
import { useTranslation } from "@/hooks/useTranslation";
import { toast } from "sonner";
import { format, parseISO } from "date-fns";
import { fr } from "date-fns/locale"; // Pour le français
import { enUS } from "date-fns/locale"; // Pour l'anglais (ajoutez si nécessaire)

function normalizeDocumentType(label: string): string {
  switch (label.toLowerCase()) {
    case "pdf":
      return "pdf";
    case "image":
      return "jpg";
    case "lab report":
      return "pdf";
    case "prescription":
      return "pdf";
    default:
      return "pdf";
  }
}

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
  nom: string;
  type: string;
  url: string;
  dateCreation: string;
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
  userRole: UserRole;
  patient: Patient;
  medicalRecord: MedicalRecord | null;
  isLoading: boolean;
  isSubmitting: boolean;
  updateMedicalRecord: (
    data: Partial<Omit<MedicalRecord, "id" | "patientId">>
  ) => Promise<void>;
  addDocument: (
    document: Omit<MedicalRecordDocument, "id" | "dateCreation">
  ) => Promise<void>;
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
  userRole,
  patient,
  medicalRecord,
  isLoading,
  isSubmitting,
  updateMedicalRecord,
  addDocument,
  deleteDocument,
  onCreateMedicalRecord,
  isCreating,
}: MedicalRecordViewProps) {
  const { t, language } = useTranslation("patients");
  const [isEditing, setIsEditing] = useState(false);
  const [formData, setFormData] = useState<Partial<MedicalRecord>>({});
  const [newDocument, setNewDocument] = useState({
    nom: "",
    type: "PDF",
    url: "",
  });
  const [showAddDocument, setShowAddDocument] = useState(false);
  const [isFormOpen, setIsFormOpen] = useState(false);

  // Déterminer le locale en fonction de la langue
  const locale = language === "fr" ? fr : enUS;

  // Formater la date avec heure
  const formatDateWithTime = (dateStr: string) => {
    return format(parseISO(dateStr), "PPP 'à' HH:mm", { locale });
  };

  // Formater la date seule (optionnel pour d'autres cas)
  const formatDateOnly = (dateStr: string) => {
    return format(parseISO(dateStr), "PPP", { locale });
  };

  // Handle form input changes
  const handleInputChange = (field: keyof MedicalRecord, value: string) => {
    setFormData((prev) => ({ ...prev, [field]: value }));
  };

  // Handle document input changes
  const handleDocumentInputChange = (
    field: keyof typeof newDocument,
    value: string
  ) => {
    setNewDocument((prev) => ({ ...prev, [field]: value }));
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
    try {
      await updateMedicalRecord(formData);
      toast.success(t("success.medical_record_updated"));
      setIsEditing(false);
    } catch (error) {
      console.error("[handleSave] Error:", error);
      toast.error(t("errors.update_medical_record_failed"));
    }
  };

  // Add a new document
  const handleAddDocument = async () => {
    if (!newDocument.nom || !newDocument.type || !newDocument.url) {
      toast.error(t("errors.document_fields_required"));
      return;
    }

    const fileExtension = normalizeDocumentType(newDocument.type);
    const filename =
      newDocument.nom.trim().toLowerCase().replace(/\s+/g, "-") +
      "." +
      fileExtension;

    const payloadFrontend = {
      nom: filename,
      type: fileExtension,
      url: newDocument.url,
    };

    try {
      await addDocument(payloadFrontend);
      toast.success(t("success.document_added"));
      setNewDocument({ nom: "", type: "PDF", url: "" });
      setShowAddDocument(false);
    } catch (error) {
      console.error("[handleAddDocument] Error:", error);
      toast.error(t("errors.add_document_failed"));
    }
  };

  if (isLoading) {
    return (
      <Card>
        <CardContent className="pt-6">
          <div className="flex justify-center items-center h-40">
            <p className="text-muted-foreground">{t("loading")}</p>
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
            <p className="text-muted-foreground mb-4">
              {t("No_medical_record_found_for_this_patient")}
            </p>
            <Button variant="default" onClick={() => setIsFormOpen(true)}>
              {t("create_medical_record")}
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
              <CardTitle>{t("medical_record")}</CardTitle>
              <CardDescription>
                {t("patient")}: {patient.name} | {t("created")}:{" "}
                {formatDateWithTime(medicalRecord.creationDate)}
              </CardDescription>
            </div>
            {userRole !== "SuperAdmin" && userRole !== "Patient" && (
              !isEditing ? (
                <Button onClick={handleEdit} variant="outline">
                  {t("edit_medical_record")}
                </Button>
              ) : (
                <div className="flex gap-2">
                  <Button onClick={() => setIsEditing(false)} variant="outline">
                    {t("cancel")}
                  </Button>
                  <Button onClick={handleSave} disabled={isSubmitting}>
                    {isSubmitting ? t("saving") : t("save_changes")}
                  </Button>
                </div>
              )
            )}
          </div>
        </CardHeader>
        <CardContent>
          <Tabs defaultValue="general" className="w-full">
            <TabsList className="grid grid-cols-2 md:grid-cols-4 mb-4">
              <TabsTrigger value="general">{t("general")}</TabsTrigger>
              <TabsTrigger value="history">{t("history")}</TabsTrigger>
              <TabsTrigger value="medications">{t("medications")}</TabsTrigger>
              <TabsTrigger value="documents">{t("documents")}</TabsTrigger>
            </TabsList>

            <TabsContent value="general">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div>
                  <h3 className="text-sm font-medium mb-2">{t("allergies")}</h3>
                  {isEditing ? (
                    <Textarea
                      value={formData.allergies || ""}
                      onChange={(e) =>
                        handleInputChange("allergies", e.target.value)
                      }
                      placeholder={t("list_patient_allergies")}
                      className="min-h-[100px]"
                    />
                  ) : (
                    <p className="text-sm text-muted-foreground whitespace-pre-wrap">
                      {medicalRecord.allergies || t("none_recorded")}
                    </p>
                  )}
                </div>
                <div>
                  <h3 className="text-sm font-medium mb-2">{t("blood_type")}</h3>
                  {isEditing ? (
                    <Select
                      value={formData.bloodType || ""}
                      onValueChange={(value) =>
                        handleInputChange("bloodType", value)
                      }
                    >
                      <SelectTrigger>
                        <SelectValue placeholder={t("select_blood_type")} />
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
                        <SelectItem value="Unknown">{t("unknown_blood_type")}</SelectItem>
                      </SelectContent>
                    </Select>
                  ) : (
                    <p className="text-sm text-muted-foreground">
                      {medicalRecord.bloodType || t("unknown_blood_type")}
                    </p>
                  )}
                </div>
                <div>
                  <h3 className="text-sm font-medium mb-2">{t("chronic_diseases")}</h3>
                  {isEditing ? (
                    <Textarea
                      value={formData.chronicDiseases || ""}
                      onChange={(e) =>
                        handleInputChange("chronicDiseases", e.target.value)
                      }
                      placeholder={t("list_chronic_conditions")}
                      className="min-h-[100px]"
                    />
                  ) : (
                    <p className="text-sm text-muted-foreground whitespace-pre-wrap">
                      {medicalRecord.chronicDiseases || t("none_recorded")}
                    </p>
                  )}
                </div>
              </div>
            </TabsContent>

            <TabsContent value="history">
              <div className="grid grid-cols-1 gap-6">
                <div>
                  <h3 className="text-sm font-medium mb-2">{t("personal_medical_history")}</h3>
                  {isEditing ? (
                    <Textarea
                      value={formData.personalHistory || ""}
                      onChange={(e) =>
                        handleInputChange("personalHistory", e.target.value)
                      }
                      placeholder={t("past_medical_procedures")}
                      className="min-h-[150px]"
                    />
                  ) : (
                    <p className="text-sm text-muted-foreground whitespace-pre-wrap">
                      {medicalRecord.personalHistory || t("no_history_recorded")}
                    </p>
                  )}
                </div>

                <div>
                  <h3 className="text-sm font-medium mb-2">{t("family_medical_history")}</h3>
                  {isEditing ? (
                    <Textarea
                      value={formData.familyHistory || ""}
                      onChange={(e) =>
                        handleInputChange("familyHistory", e.target.value)
                      }
                      placeholder={t("family_medical_history_details")}
                      className="min-h-[150px]"
                    />
                  ) : (
                    <p className="text-sm text-muted-foreground whitespace-pre-wrap">
                      {medicalRecord.familyHistory || t("no_family_history_recorded")}
                    </p>
                  )}
                </div>
              </div>
            </TabsContent>

            <TabsContent value="medications">
              <div>
                <h3 className="text-sm font-medium mb-2">{t("current_medications")}</h3>
                {isEditing ? (
                  <Textarea
                    value={formData.currentMedications || ""}
                    onChange={(e) =>
                      handleInputChange("currentMedications", e.target.value)
                    }
                    placeholder={t("list_current_medications")}
                    className="min-h-[200px]"
                  />
                ) : (
                  <p className="text-sm text-muted-foreground whitespace-pre-wrap">
                    {medicalRecord.currentMedications || t("no_medications_recorded")}
                  </p>
                )}
              </div>
            </TabsContent>

            <TabsContent value="documents">
              <div className="space-y-4">
                <div className="flex justify-between items-center">
                  <h3 className="text-sm font-medium">{t("medical_documents")}</h3>
                  {userRole !== "SuperAdmin" && userRole !== "Patient" && (
                    <Button
                      variant="outline"
                      size="sm"
                      onClick={() => setShowAddDocument(!showAddDocument)}
                    >
                      <Plus className="mr-1" size={16} /> {t("add_document")}
                    </Button>
                  )}
                </div>

                {showAddDocument && (
                  <Card className="border border-dashed p-4">
                    <div className="space-y-3">
                      <Input
                        placeholder={t("document_name")}
                        value={newDocument.nom}
                        onChange={(e) =>
                          handleDocumentInputChange("nom", e.target.value)
                        }
                      />
                      <Select
                        value={newDocument.type}
                        onValueChange={(value) =>
                          handleDocumentInputChange("type", value)
                        }
                      >
                        <SelectTrigger>
                          <SelectValue placeholder={t("select_document_type")} />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectItem value="PDF">{t("pdf")}</SelectItem>
                          <SelectItem value="Image">{t("image")}</SelectItem>
                          <SelectItem value="Lab Report">{t("lab_report")}</SelectItem>
                          <SelectItem value="Prescription">{t("prescription")}</SelectItem>
                          <SelectItem value="Other">{t("other_document")}</SelectItem>
                        </SelectContent>
                      </Select>
                      <Input
                        placeholder={t("document_url")}
                        value={newDocument.url}
                        onChange={(e) =>
                          handleDocumentInputChange("url", e.target.value)
                        }
                      />
                      <div className="flex justify-end gap-2">
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={() => setShowAddDocument(false)}
                        >
                          {t("cancel")}
                        </Button>
                        <Button
                          size="sm"
                          onClick={handleAddDocument}
                          disabled={!newDocument.nom}
                        >
                          {t("add_document")}
                        </Button>
                      </div>
                    </div>
                  </Card>
                )}

                {medicalRecord.documents.length === 0 ? (
                  <p className="text-sm text-muted-foreground py-4">
                    {t("no_documents_available")}
                  </p>
                ) : (
                  <div className="border rounded-md overflow-hidden">
                    <table className="w-full">
                      <thead>
                        <tr className="bg-muted/50">
                          <th className="text-left p-2 text-sm font-medium">{t("name")}</th>
                          <th className="text-left p-2 text-sm font-medium">{t("type")}</th>
                          <th className="text-left p-2 text-sm font-medium">{t("upload_date")}</th>
                          <th className="text-right p-2 text-sm font-medium">{t("actions")}</th>
                        </tr>
                      </thead>
                      <tbody>
                        {medicalRecord.documents.map((doc) => (
                          <tr key={doc.id} className="border-t">
                            <td className="p-2 text-sm">
                              <div className="flex items-center">
                                <FileText
                                  size={16}
                                  className="mr-2 text-muted-foreground"
                                />
                                {doc.nom}
                              </div>
                            </td>
                            <td className="p-2 text-sm">{t(doc.type.toLowerCase())}</td>
                            <td className="p-2 text-sm">{formatDateWithTime(doc.dateCreation)}</td>
                            <td className="p-2 text-right">
                              <div className="flex justify-end gap-1">
                                {doc.url && (
                                  <Button variant="ghost" size="sm" aria-label={t("view_document")}>
                                    <Eye size={16} />
                                  </Button>
                                )}
                                {userRole === "ClinicAdmin" && (
                                  <Button
                                    variant="ghost"
                                    size="sm"
                                    onClick={() => deleteDocument(doc.id)}
                                    aria-label={t("delete_document")}
                                  >
                                    <Trash2
                                      size={16}
                                      className="text-red-500"
                                    />
                                  </Button>
                                )}
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