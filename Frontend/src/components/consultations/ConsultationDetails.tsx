import { useState, useMemo } from "react";
import { useForm } from "react-hook-form";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
  DialogFooter,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Calendar,
  Clock,
  FileText,
  User,
  Paperclip,
  Upload,
  Trash2,
  Download,
  Image as ImageIcon,
  FileIcon,
  Eye,
} from "lucide-react";
import { toast } from "sonner";
import { format, parseISO } from "date-fns";
import { fr } from "date-fns/locale";
import {
  Consultation,
  DocumentMedicalDTO,
  ConsultationType,
  consultationTypes,
} from "@/types/consultation";
import { useConsultations } from "@/hooks/useConsultations";
import { useAuth } from "@/hooks/useAuth";
import { useTranslation } from "@/hooks/useTranslation";
import { cn } from "@/lib/utils";
import { ConfirmDeleteDialog } from "./ConfirmDeleteDialog";

interface MedicalDocument {
  id?: string;
  consultationId: string;
  fileName: string;
  type: string;
  fichierURL: string;
  dateAjout?: string;
}

interface FileUploadForm {
  files: FileList;
}

interface ConsultationDetailsProps {
  isOpen: boolean;
  onClose: () => void;
  consultation: Consultation | null;
  patientName: string;
  doctorName: string;
}

export function ConsultationDetails({
  isOpen,
  onClose,
  consultation,
  patientName,
  doctorName,
}: ConsultationDetailsProps) {
  const { t } = useTranslation("consultations");
  const { user } = useAuth();
  const { uploadDocumentMedical, deleteDocumentMedical } = useConsultations();
  const [showCompleteView, setShowCompleteView] = useState(false);
  const [previewFile, setPreviewFile] = useState<MedicalDocument | null>(null);
  const [isUploading, setIsUploading] = useState(false);
  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
  const [documentToDelete, setDocumentToDelete] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<FileUploadForm>();

  // Color mapping for consultation types
  const consultationTypeColors: Record<ConsultationType, string> = {
    [ConsultationType.ConsultationGenerale]: "bg-blue-100 text-blue-800",
    [ConsultationType.ConsultationSpecialiste]: "bg-green-100 text-green-800",
    [ConsultationType.ConsultationUrgence]: "bg-red-100 text-red-800",
    [ConsultationType.ConsultationSuivi]: "bg-purple-100 text-purple-800",
    [ConsultationType.ConsultationLaboratoire]: "bg-yellow-100 text-yellow-800",
  };

  // Move useMemo before any early returns
  const formattedDate = useMemo(() => {
    return consultation
      ? format(parseISO(consultation.dateConsultation), "PPPP, HH:mm", {
          locale: fr,
        })
      : "";
  }, [consultation]);

  if (!consultation) return null;

  const isDoctor = user?.role === "Doctor";
  const documents = consultation.documents || [];

  // File type utilities
  const getFileType = (fileName: string): "image" | "pdf" | "other" => {
    const ext = fileName.toLowerCase().split(".").pop();
    if (["jpg", "jpeg", "png", "gif", "bmp", "webp"].includes(ext || "")) {
      return "image";
    }
    if (ext === "pdf") {
      return "pdf";
    }
    return "other";
  };

  const getFileIcon = (fileName: string) => {
    const type = getFileType(fileName);
    switch (type) {
      case "image":
        return <ImageIcon className="h-5 w-5 text-muted-foreground" />;
      case "pdf":
        return <FileText className="h-5 w-5 text-muted-foreground" />;
      default:
        return <FileIcon className="h-5 w-5 text-muted-foreground" />;
    }
  };

  // File handling functions
  const handlePreviewFile = (document: MedicalDocument) => {
    const fileType = getFileType(document.fileName);
    if (fileType === "image" || fileType === "pdf") {
      setPreviewFile(document);
    } else {
      window.open(document.fichierURL, "_blank");
    }
  };

  const handleDownloadFile = (doc: MedicalDocument) => {
    const link = document.createElement("a");
    link.href = doc.fichierURL;
    link.download = doc.fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  };

  const handleDeleteDocument = (documentId: string) => {
    setDocumentToDelete(documentId);
    setIsDeleteDialogOpen(true);
  };

  const handleConfirmDeleteDocument = async () => {
    if (documentToDelete) {
      try {
        await deleteDocumentMedical(documentToDelete);
      } catch (error) {
        // Error toast is handled in the hook
      }
    }
    setIsDeleteDialogOpen(false);
    setDocumentToDelete(null);
  };

  const onSubmitFiles = async (data: FileUploadForm) => {
    if (!data.files || data.files.length === 0) {
      toast.error(t("errorAddingDocument"));
      return;
    }

    setIsUploading(true);
    try {
      for (const file of Array.from(data.files)) {
        await uploadDocumentMedical(consultation.id!, file);
      }
      toast.success(t("documentAddSuccess"));
      reset();
    } catch (error) {
      toast.error(t("errorAddingDocument"));
    } finally {
      setIsUploading(false);
    }
  };

  return (
    <>
      <Dialog open={isOpen} onOpenChange={onClose}>
        <DialogContent
          className={cn(
            "sm:max-w-[700px] max-h-[90vh] overflow-y-auto",
            showCompleteView && "sm:max-w-[900px]"
          )}
        >
          <DialogHeader>
            <DialogTitle>{t("consultationDetailsTitle")}</DialogTitle>
            <DialogDescription>
              {showCompleteView
                ? t("consultationDetailsComplete")
                : t("consultationDetailsSummary")}
            </DialogDescription>
          </DialogHeader>

          <div className="space-y-6 py-4">
            {/* Consultation Information */}
            <Card>
              <CardHeader>
                <CardTitle className="text-lg">
                  {t("generalInformation")}
                </CardTitle>
              </CardHeader>
              <CardContent className="grid gap-4 sm:grid-cols-2">
                <div>
                  <div className="flex items-center gap-2 mb-1">
                    <Calendar className="h-4 w-4 text-muted-foreground" />
                    <span className="font-medium">{t("dateAndTime")}</span>
                  </div>
                  <p className="text-sm">{formattedDate}</p>
                </div>
                <div>
                  <div className="flex items-center gap-2 mb-1">
                    <User className="h-4 w-4 text-muted-foreground" />
                    <span className="font-medium">{t("patient")}</span>
                  </div>
                  <p className="text-sm">{patientName || t("notSpecified")}</p>
                </div>
                <div>
                  <div className="flex items-center gap-2 mb-1">
                    <User className="h-4 w-4 text-muted-foreground" />
                    <span className="font-medium">{t("doctor")}</span>
                  </div>
                  <p className="text-sm">{doctorName || t("notSpecified")}</p>
                </div>
                {consultation.type && (
                  <div>
                    <div className="flex items-center gap-2 mb-1">
                      <FileText className="h-4 w-4 text-muted-foreground" />
                      <span className="font-medium">
                        {t("typeConsultation")}
                      </span>
                    </div>
                    <Badge
                      className={cn(consultationTypeColors[consultation.type])}
                    >
                      {t(consultationTypes[consultation.type])}
                    </Badge>
                  </div>
                )}
              </CardContent>
            </Card>

            {/* Diagnostic and Notes */}
            {(consultation.diagnostic || consultation.notes) && (
              <Card>
                <CardHeader>
                  <CardTitle className="text-lg">
                    {t("medicalDetails")}
                  </CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                  {consultation.diagnostic && (
                    <div>
                      <div className="flex items-center gap-2 mb-2">
                        <FileText className="h-4 w-4 text-muted-foreground" />
                        <span className="font-medium">{t("diagnostic")}</span>
                      </div>
                      <p className="text-sm p-3 bg-muted/30 rounded-md">
                        {consultation.diagnostic}
                      </p>
                    </div>
                  )}
                  {consultation.notes && showCompleteView && (
                    <div>
                      <div className="flex items-center gap-2 mb-2">
                        <FileText className="h-4 w-4 text-muted-foreground" />
                        <span className="font-medium">{t("notes")}</span>
                      </div>
                      <p className="text-sm p-3 bg-muted/30 rounded-md whitespace-pre-wrap">
                        {consultation.notes}
                      </p>
                    </div>
                  )}
                </CardContent>
              </Card>
            )}

            {/* Documents Section - Only in Complete View */}
            {showCompleteView && (
              <Card>
                <CardHeader>
                  <CardTitle className="text-lg flex items-center gap-2">
                    <Paperclip className="h-4 w-4 text-muted-foreground" />
                    {t("documents")} ({documents.length})
                  </CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                  {isDoctor && (
                    <div className="border p-4 rounded-md bg-muted/20">
                      <form
                        onSubmit={handleSubmit(onSubmitFiles)}
                        className="space-y-3"
                      >
                        <div>
                          <Label htmlFor="files">{t("addDocuments")}</Label>
                          <Input
                            id="files"
                            type="file"
                            multiple
                            accept=".pdf,.jpg,.jpeg,.png,.gif,.bmp,.webp,.doc,.docx"
                            {...register("files", {
                              required: t("errorAddingDocument"),
                            })}
                            className="cursor-pointer"
                          />
                          {errors.files && (
                            <p className="text-sm text-red-500 mt-1">
                              {errors.files.message}
                            </p>
                          )}
                        </div>
                        <Button
                          type="submit"
                          size="sm"
                          disabled={isUploading}
                          className="w-full"
                        >
                          <Upload className="mr-2 h-4 w-4" />
                          {isUploading ? t("uploading") : t("upload")}
                        </Button>
                      </form>
                    </div>
                  )}

                  {documents.length === 0 ? (
                    <p className="text-sm text-muted-foreground text-center py-4">
                      {t("noDocumentsAttached")}
                    </p>
                  ) : (
                    <div className="grid gap-2">
                      {documents.map((document) => (
                        <div
                          key={document.id}
                          className="flex items-center justify-between p-3 bg-muted/30 rounded-md hover:bg-muted/50 transition"
                        >
                          <div className="flex items-center gap-3">
                            {getFileIcon(document.fileName)}
                            <div>
                              <p className="text-sm font-medium">
                                {document.fileName}
                              </p>
                              <p className="text-xs text-muted-foreground">
                                {document.dateAjout
                                  ? format(parseISO(document.dateAjout), "PP", {
                                      locale: fr,
                                    })
                                  : t("unknownDate")}
                              </p>
                            </div>
                          </div>
                          <div className="flex items-center gap-2">
                            <Button
                              size="sm"
                              variant="ghost"
                              onClick={() => handlePreviewFile(document)}
                            >
                              <Eye className="h-4 w-4" />
                            </Button>
                            <Button
                              size="sm"
                              variant="ghost"
                              onClick={() => handleDownloadFile(document)}
                            >
                              <Download className="h-4 w-4" />
                            </Button>
                            {isDoctor && (
                              <Button
                                size="sm"
                                variant="ghost"
                                className="text-red-500 hover:text-red-700"
                                onClick={() =>
                                  handleDeleteDocument(document.id!)
                                }
                              >
                                <Trash2 className="h-4 w-4" />
                              </Button>
                            )}
                          </div>
                        </div>
                      ))}
                    </div>
                  )}
                </CardContent>
              </Card>
            )}
          </div>

          <DialogFooter className="flex justify-between sm:justify-end gap-2">
            <Button
              variant="outline"
              onClick={() => setShowCompleteView(!showCompleteView)}
            >
              {showCompleteView ? t("simplifiedView") : t("completeView")}
            </Button>
            <Button onClick={onClose}>{t("close")}</Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* Preview Dialog */}
      {previewFile && (
        <Dialog open={!!previewFile} onOpenChange={() => setPreviewFile(null)}>
          <DialogContent className="sm:max-w-[800px] sm:max-h-[80vh]">
            <DialogHeader>
              <DialogTitle>{previewFile.fileName}</DialogTitle>
              <DialogDescription>{t("documentPreview")}</DialogDescription>
            </DialogHeader>
            <div className="flex justify-center items-center h-[60vh] bg-muted/30 rounded-md overflow-hidden">
              {getFileType(previewFile.fileName) === "image" ? (
                <img
                  src={previewFile.fichierURL}
                  alt={previewFile.fileName}
                  className="max-w-full max-h-full object-contain"
                />
              ) : getFileType(previewFile.fileName) === "pdf" ? (
                <iframe
                  src={previewFile.fichierURL}
                  className="w-full h-full border-0"
                  title={previewFile.fileName}
                />
              ) : (
                <p className="text-muted-foreground">
                  {t("previewNotAvailable")}
                </p>
              )}
            </div>
            <DialogFooter>
              <Button
                variant="outline"
                onClick={() => handleDownloadFile(previewFile)}
              >
                <Download className="mr-2 h-4 w-4" /> {t("download")}
              </Button>
              <Button onClick={() => setPreviewFile(null)}>{t("close")}</Button>
            </DialogFooter>
          </DialogContent>
        </Dialog>
      )}

      {isDeleteDialogOpen && (
        <ConfirmDeleteDialog
          isOpen={isDeleteDialogOpen}
          onClose={() => {
            setIsDeleteDialogOpen(false);
            setDocumentToDelete(null);
          }}
          onConfirm={handleConfirmDeleteDocument}
          title={t("confirmDeleteDocument")}
          message={t("confirmDeleteDocumentDescription")}
        />
      )}
    </>
  );
}
