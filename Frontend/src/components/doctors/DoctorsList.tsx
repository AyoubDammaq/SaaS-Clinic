import { useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Search, Plus, FileEdit, Trash2 } from "lucide-react";
import { Card, CardContent } from "@/components/ui/card";
import { Doctor } from "@/types/doctor";
import { useAuth } from "@/hooks/useAuth";
import { useTranslation } from "@/hooks/useTranslation";

interface DoctorsListProps {
  doctors: Doctor[];
  filteredDoctors: Doctor[];
  searchTerm: string;
  setSearchTerm: (term: string) => void;
  isLoading: boolean;
  permissions: {
    canCreate: boolean;
    canEdit: boolean;
    canDelete: boolean;
  };
  onAddDoctor: () => void;
  onEditDoctor: (doctor: Doctor) => void;
  onDeleteDoctor: (id: string) => void;
}

export function DoctorsList({
  doctors,
  filteredDoctors,
  searchTerm,
  setSearchTerm,
  isLoading,
  permissions,
  onAddDoctor,
  onEditDoctor,
  onDeleteDoctor,
}: DoctorsListProps) {
  const navigate = useNavigate();
  const { user } = useAuth();
  const { t } = useTranslation("doctors");

  const handleRowClick = (doctor: Doctor) => {
    navigate(`/doctors/${doctor.id}`);
  };

  function getTranslatedSpecialty(
    specialty: string,
    t: (key: string) => string
  ): string {
    const match = specialties.find((s) => s.value === specialty);
    return t(match?.key || specialty);
  }

  // Définir les valeurs brutes des spécialités (pour compatibilité avec la base de données)
  const specialties = [
    { value: "General Practitioner", key: "generalPractitioner" },
    { value: "Pediatrician", key: "pediatrician" },
    { value: "Cardiologist", key: "cardiologist" },
    { value: "Dermatologist", key: "dermatologist" },
    { value: "Neurologist", key: "neurologist" },
    { value: "Psychiatrist", key: "psychiatrist" },
    { value: "Ophthalmologist", key: "ophthalmologist" },
    { value: "Gynecologist", key: "gynecologist" },
    { value: "Orthopedist", key: "orthopedist" },
    { value: "Dentist", key: "dentist" },
  ];

  return (
    <div>
      <div className="flex items-center justify-between mb-4">
        <div className="relative w-full max-w-sm">
          <Search className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
          <Input
            placeholder={t("searchDoctors")}
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="pl-8"
          />
        </div>
        {permissions.canCreate && (
          <Button className="ml-2" onClick={onAddDoctor}>
            <Plus className="mr-1 h-4 w-4" /> {t("addDoctor")}
          </Button>
        )}
      </div>

      <div className="rounded-md border">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>
                {t("fullName")}
              </TableHead>
              <TableHead>{t("specialty")}</TableHead>
              <TableHead>{t("email")}</TableHead>
              <TableHead>{t("phone")}</TableHead>
              <TableHead>{t("clinic")}</TableHead>
              {(permissions.canEdit || permissions.canDelete) && (
                <TableHead>{t("actions")}</TableHead>
              )}
            </TableRow>
          </TableHeader>
          <TableBody>
            {filteredDoctors.length === 0 ? (
              <TableRow>
                <TableCell colSpan={6} className="h-24 text-center">
                  {t("noDoctorsFound")}
                </TableCell>
              </TableRow>
            ) : (
              filteredDoctors.map((doctor) => {
                return (
                  <TableRow
                    key={doctor.id}
                    className="cursor-pointer hover:bg-muted/60 focus:outline-none focus:ring-2 focus:ring-primary"
                    onClick={() => handleRowClick(doctor)}
                  >
                    <TableCell className="font-medium">
                      {doctor.prenom} {doctor.nom}
                    </TableCell>
                    <TableCell>
                      <TableCell>
                        {getTranslatedSpecialty(doctor.specialite, t)}
                      </TableCell>
                    </TableCell>
                    <TableCell>{doctor.email}</TableCell>
                    <TableCell>{doctor.telephone}</TableCell>
                    <TableCell>
                      {doctor.cliniqueId ? t("assigned") : t("notAssigned")}
                    </TableCell>
                    {(permissions.canEdit || permissions.canDelete) && (
                      <TableCell onClick={(e) => e.stopPropagation()}>
                        <div className="flex items-center gap-2">
                          {permissions.canEdit && (
                            <Button
                              size="sm"
                              variant="ghost"
                              onClick={(e) => {
                                e.stopPropagation();
                                onEditDoctor(doctor);
                              }}
                              aria-label={t("editDoctor")}
                            >
                              <FileEdit className="h-4 w-4" />
                            </Button>
                          )}
                          {permissions.canDelete && (
                            <Button
                              size="sm"
                              variant="ghost"
                              className="text-red-500"
                              onClick={(e) => {
                                e.stopPropagation();
                                onDeleteDoctor(doctor.id);
                              }}
                              aria-label={t("deleteDoctor")}
                            >
                              <Trash2 className="h-4 w-4" />
                            </Button>
                          )}
                        </div>
                      </TableCell>
                    )}
                  </TableRow>
                );
              })
            )}
          </TableBody>
        </Table>
      </div>
    </div>
  );
}
