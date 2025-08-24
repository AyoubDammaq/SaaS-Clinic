import { useState, useEffect, useCallback } from "react";
import { User } from "@/types/auth";
import { useAuth } from "@/hooks/useAuth";

export function useUsers() {
  const { user, getAllUsers, deleteUser, changeUserRole, registerWithDefaultPassword } = useAuth();

  const [users, setUsers] = useState<User[]>([]);
  const [filteredUsers, setFilteredUsers] = useState<User[]>([]);
  const [searchTerm, setSearchTerm] = useState("");
  const [isLoading, setIsLoading] = useState(false);

  // Définir les permissions selon le rôle connecté
  const permissions = {
    canCreate: user?.role === "SuperAdmin" || user?.role === "ClinicAdmin",
    canEdit: user?.role === "SuperAdmin" || user?.role === "ClinicAdmin",
    canDelete: user?.role === "SuperAdmin",
  };

  // Charger tous les utilisateurs depuis l'API
  const fetchUsers = useCallback(async () => {
    setIsLoading(true);
    try {
      const result = await getAllUsers();
      if (result) {
        setUsers(result);
        setFilteredUsers(result);
      }
    } catch (error) {
      console.error("Failed to fetch users:", error);
    } finally {
      setIsLoading(false);
    }
  }, [getAllUsers]);

  useEffect(() => {
    fetchUsers();
  }, [fetchUsers]);

  // Filtrer les utilisateurs selon le searchTerm
  useEffect(() => {
    if (!searchTerm) {
      setFilteredUsers(users);
    } else {
      const term = searchTerm.toLowerCase();
      setFilteredUsers(
        users.filter(
          (u) =>
            u.name.toLowerCase().includes(term) ||
            u.email.toLowerCase().includes(term) ||
            u.role.toLowerCase().includes(term)
        )
      );
    }
  }, [searchTerm, users]);

  // Ajouter un utilisateur
  const handleAddUser = async (data: Omit<User, "id">): Promise<User> => {
    try {
      const newUser = await registerWithDefaultPassword(
        data.name,
        data.email,
        data.role
      );
      if (newUser) {
        setUsers((prev) => [newUser, ...prev]);
        setFilteredUsers((prev) => [newUser, ...prev]);
        return newUser;
      }
      throw new Error("Failed to create user");
    } catch (error) {
      console.error(error);
      throw error;
    }
  };

  // Mettre à jour un utilisateur
  const handleUpdateUser = async (userId: string, data: Partial<User>): Promise<User> => {
    try {
      if (data.role) {
        await changeUserRole(userId, data.role);
      }
      const updatedUser: User = { ...users.find((u) => u.id === userId)!, ...data };
      setUsers((prev) => prev.map((u) => (u.id === userId ? updatedUser : u)));
      setFilteredUsers((prev) => prev.map((u) => (u.id === userId ? updatedUser : u)));
      return updatedUser;
    } catch (error) {
      console.error("Failed to update user:", error);
      throw error;
    }
  };

  // Supprimer un utilisateur
  const handleDeleteUser = async (userId: string): Promise<void> => {
    try {
      await deleteUser(userId);
      setUsers((prev) => prev.filter((u) => u.id !== userId));
      setFilteredUsers((prev) => prev.filter((u) => u.id !== userId));
    } catch (error) {
      console.error("Failed to delete user:", error);
      throw error;
    }
  };

  return {
    users,
    filteredUsers,
    searchTerm,
    setSearchTerm,
    isLoading,
    permissions,
    handleAddUser,
    handleUpdateUser,
    handleDeleteUser,
  };
}
