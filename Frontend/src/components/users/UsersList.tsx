import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Search, Plus, FileEdit, Trash2 } from "lucide-react";
import { Card, CardContent } from "@/components/ui/card";
import { roleMap, User, UserRole } from "@/types/auth";
import { useTranslation } from "@/hooks/useTranslation";

interface UsersListProps {
  users: User[];
  filteredUsers: User[];
  userRole: UserRole;
  searchTerm: string;
  setSearchTerm: (term: string) => void;
  isLoading: boolean;
  permissions: {
    canCreate: boolean;
    canEdit: boolean;
    canDelete: boolean;
  };
  onAddUser: () => void;
  onEditUser: (user: User) => void;
  onDeleteUser: (id: string) => void;
}

export function UsersList({
  filteredUsers,
  userRole,
  searchTerm,
  setSearchTerm,
  isLoading,
  permissions,
  onAddUser,
  onEditUser,
  onDeleteUser,
}: UsersListProps) {
  const { t } = useTranslation("users");

  if (isLoading) {
    return (
      <Card>
        <CardContent className="pt-6">
          <div className="flex justify-center items-center h-40">
            <p className="text-muted-foreground">{t("loading_users")}</p>
          </div>
        </CardContent>
      </Card>
    );
  }

  return (
    <div>
      <div className="flex items-center justify-between mb-4">
        <div className="relative w-full max-w-sm">
          <Search className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
          <Input
            placeholder={t("search_users_placeholder")}
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="pl-8"
          />
        </div>
        {/* {permissions.canCreate && (
          <Button className="ml-2" onClick={onAddUser}>
            <Plus className="mr-1 h-4 w-4" /> {t("add_user")}
          </Button>
        )} */}
      </div>

      <div className="rounded-md border">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>{t("full_name")}</TableHead>
              <TableHead>{t("email")}</TableHead>
              <TableHead>{t("role")}</TableHead>
              {(permissions.canEdit || permissions.canDelete) && <TableHead>{t("actions")}</TableHead>}
            </TableRow>
          </TableHeader>
          <TableBody>
            {filteredUsers.length === 0 ? (
              <TableRow>
                <TableCell colSpan={4} className="h-24 text-center">
                  {t("no_users_found")}
                </TableCell>
              </TableRow>
            ) : (
              filteredUsers.map((user) => (
                <TableRow key={user.id} className="hover:bg-muted/60">
                  <TableCell className="font-medium">{user.fullName || user.name}</TableCell>
                  <TableCell>{user.email}</TableCell>
                  <TableCell>{roleMap[Number(user.role)]}</TableCell>
                  {(permissions.canEdit || permissions.canDelete) && (
                    <TableCell>
                      <div className="flex items-center gap-2">
                        {/* {permissions.canEdit && (
                          <Button size="sm" variant="ghost" onClick={() => onEditUser(user)}>
                            <FileEdit className="h-4 w-4" />
                          </Button>
                        )} */}
                        {permissions.canDelete && (
                          <Button size="sm" variant="ghost" className="text-red-500" onClick={() => onDeleteUser(user.id)}>
                            <Trash2 className="h-4 w-4" />
                          </Button>
                        )}
                      </div>
                    </TableCell>
                  )}
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </div>
    </div>
  );
}
