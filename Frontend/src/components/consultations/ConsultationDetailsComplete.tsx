import { useState } from "react";
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
import { consultationService } from "@/services/consultationService";
import { DocumentMedicalDTO } from "@/types/consultation";
import { useConsultations } from "@/hooks/useConsultations";
import { useAuth } from "@/hooks/useAuth";
import { is } from "date-fns/locale";

interface MedicalDocument {
  id?: string;
  consultationId: string;
  fileName: string;
  type: string;
  fichierURL: string;
  dateAjout?: string;
}

interface Consultation {
  id?: string;
  patientId: string;
  medecinId: string;
  dateConsultation: string;
  diagnostic?: string;
  notes?: string;
  documents?: MedicalDocument[];
}

interface ConsultationDetailsCompleteProps {
  isOpen: boolean;
  onClose: () => void;
  consultation: Consultation | null;
  patientName: string;
  doctorName: string;
  onUpdate?: (updatedConsultation: Consultation) => void;
}

interface FileUploadForm {
  files: FileList;
}

export function ConsultationDetailsComplete({
  isOpen,
  onClose,
  consultation,
  patientName,
  doctorName,
  onUpdate,
}: ConsultationDetailsCompleteProps) {
  const { user } = useAuth();
  const isDoctor = user?.role === "Doctor";
  const { uploadDocumentMedical, deleteDocumentMedical } = useConsultations();
  const [isUploading, setIsUploading] = useState(false);
  const [attachments, setAttachments] = useState<MedicalDocument[]>(
    consultation?.documents || []
  );
  const [previewFile, setPreviewFile] = useState<MedicalDocument | null>(null);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<FileUploadForm>();

  if (!consultation) return null;

  // Fonction pour déterminer le type de fichier
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

  // Fonction pour obtenir l'icône appropriée
  const getFileIcon = (fileName: string) => {
    const type = getFileType(fileName);
    switch (type) {
      case "image":
        return <ImageIcon className="h-5 w-5" />;
      case "pdf":
        return <FileText className="h-5 w-5" />;
      default:
        return <FileIcon className="h-5 w-5" />;
    }
  };

  // Upload de fichiers
  const onSubmitFiles = async (data: FileUploadForm) => {
    if (!data.files || data.files.length === 0) {
      toast.error("Veuillez sélectionner au moins un fichier");
      return;
    }

    try {
      const uploadedDocuments: MedicalDocument[] = [];
      for (const file of Array.from(data.files)) {
        await uploadDocumentMedical(consultation!.id!, file);
      }
      toast.success(
        `${uploadedDocuments.length} fichier(s) uploadé(s) avec succès`
      );
      reset();
    } catch (error) {
      console.error("Erreur lors de l'upload des fichiers", error);
      toast.error("Erreur lors de l'upload des fichiers");
    }
  };

  // Suppression d'un document
  const handleDeleteDocument = async (documentId: string) => {
    if (!window.confirm("Êtes-vous sûr de vouloir supprimer ce document ?"))
      return;

    try {
      await deleteDocumentMedical(documentId);

      const updatedAttachments = attachments.filter(
        (doc) => doc.id !== documentId
      );
      setAttachments(updatedAttachments);

      if (onUpdate) {
        onUpdate({
          ...consultation!,
          documents: updatedAttachments,
        });
      }

      toast.success("Document supprimé avec succès");
    } catch (error) {
      console.error("Erreur lors de la suppression du document", error);
      toast.error("Erreur lors de la suppression du document");
    }
  };

  // Prévisualisation d'un fichier
  const handlePreviewFile = (document: MedicalDocument) => {
    const fileType = getFileType(document.fileName);
    if (fileType === "image" || fileType === "pdf") {
      setPreviewFile(document);
    } else {
      // Pour les autres types, ouvrir directement le lien
      window.open(document.fichierURL, "_blank");
    }
  };

  // Téléchargement d'un fichier
  const handleDownloadFile = (doc: MedicalDocument) => {
    const link = document.createElement("a");
    link.href = doc.fichierURL;
    link.download = doc.fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  };

  return (
    <>
      <Dialog open={isOpen} onOpenChange={onClose}>
        <DialogContent className="sm:max-w-[700px] max-h-[90vh] overflow-y-auto">
          <DialogHeader>
            <DialogTitle>Détails de la consultation</DialogTitle>
            <DialogDescription>
              Informations complètes et documents attachés
            </DialogDescription>
          </DialogHeader>

          <div className="space-y-6 py-4">
            {/* Informations générales */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <h3 className="font-medium flex items-center gap-2">
                  <User className="h-4 w-4 text-muted-foreground" />
                  Patient
                </h3>
                <p className="text-sm">{patientName || "Non spécifié"}</p>
              </div>
              <div>
                <h3 className="font-medium flex items-center gap-2">
                  <User className="h-4 w-4 text-muted-foreground" />
                  Médecin
                </h3>
                <p className="text-sm">{doctorName || "Non spécifié"}</p>
              </div>
            </div>

            <div>
              <h3 className="font-medium flex items-center gap-2">
                <Calendar className="h-4 w-4 text-muted-foreground" />
                Date et heure
              </h3>
              <p className="text-sm">{consultation.dateConsultation}</p>
            </div>

            {consultation.diagnostic && (
              <div>
                <h3 className="font-medium">Diagnostic</h3>
                <p className="text-sm whitespace-pre-wrap border p-3 rounded-md bg-muted/30">
                  {consultation.diagnostic}
                </p>
              </div>
            )}

            {consultation.notes && (
              <div>
                <h3 className="font-medium">Notes complémentaires</h3>
                <p className="text-sm whitespace-pre-wrap border p-3 rounded-md bg-muted/30">
                  {consultation.notes}
                </p>
              </div>
            )}

            {/* Section des documents attachés */}
            <div className="space-y-4">
              <div className="flex items-center justify-between">
                <h3 className="font-medium flex items-center gap-2">
                  <Paperclip className="h-4 w-4 text-muted-foreground" />
                  Documents médicaux ({attachments.length})
                </h3>
              </div>

              {/* Upload de nouveaux fichiers */}
              {isDoctor && (
                <Card>
                  <CardHeader className="pb-3">
                    <CardTitle className="text-sm">
                      Ajouter des documents
                    </CardTitle>
                  </CardHeader>
                  <CardContent>
                    <form
                      onSubmit={handleSubmit(onSubmitFiles)}
                      className="space-y-3"
                    >
                      <div>
                        <Label htmlFor="files">Sélectionner des fichiers</Label>
                        <Input
                          id="files"
                          type="file"
                          multiple
                          accept=".pdf,.jpg,.jpeg,.png,.gif,.bmp,.webp,.doc,.docx"
                          {...register("files", {
                            required:
                              "Veuillez sélectionner au moins un fichier",
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
                        {isUploading
                          ? "Upload en cours..."
                          : "Uploader les fichiers"}
                      </Button>
                    </form>
                  </CardContent>
                </Card>
              )}

              {/* Liste des documents */}
              <div className="space-y-2">
                {attachments.length === 0 ? (
                  <p className="text-sm text-muted-foreground text-center py-8">
                    Aucun document attaché à cette consultation
                  </p>
                ) : (
                  attachments.map((document) => (
                    <Card
                      key={document.id}
                      className="hover:shadow-md transition-shadow"
                    >
                      <CardContent className="p-4">
                        <div className="flex items-center justify-between">
                          <div className="flex items-center space-x-3">
                            {getFileIcon(document.fileName)}
                            <div>
                              <p className="text-sm font-medium">
                                {document.fileName}
                              </p>
                              <p className="text-xs text-muted-foreground">
                                Uploadé le{" "}
                                {new Date(
                                  document.dateAjout
                                ).toLocaleDateString("fr-FR")}
                              </p>
                            </div>
                          </div>
                          <div className="flex items-center space-x-2">
                            <Button
                              size="sm"
                              variant="outline"
                              onClick={() => handlePreviewFile(document)}
                            >
                              <Eye className="h-4 w-4" />
                            </Button>
                            <Button
                              size="sm"
                              variant="outline"
                              onClick={() => handleDownloadFile(document)}
                            >
                              <Download className="h-4 w-4" />
                            </Button>
                            {isDoctor && (
                              <Button
                                size="sm"
                                variant="outline"
                                className="text-red-500 hover:text-red-700"
                                onClick={() =>
                                  handleDeleteDocument(document.id)
                                }
                              >
                                <Trash2 className="h-4 w-4" />
                              </Button>
                            )}
                          </div>
                        </div>
                      </CardContent>
                    </Card>
                  ))
                )}
              </div>
            </div>
          </div>

          <DialogFooter>
            <Button onClick={onClose}>Fermer</Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* Dialog de prévisualisation */}
      {previewFile && (
        <Dialog open={!!previewFile} onOpenChange={() => setPreviewFile(null)}>
          <DialogContent className="sm:max-w-[800px] sm:max-h-[600px]">
            <DialogHeader>
              <DialogTitle>Aperçu - {previewFile.fileName}</DialogTitle>
            </DialogHeader>
            <div className="flex justify-center items-center h-96 bg-muted/30 rounded-md">
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
                  Aperçu non disponible pour ce type de fichier
                </p>
              )}
            </div>
            <DialogFooter>
              <Button onClick={() => setPreviewFile(null)}>Fermer</Button>
            </DialogFooter>
          </DialogContent>
        </Dialog>
      )}
    </>
  );
}
