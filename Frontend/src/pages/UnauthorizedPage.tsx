import { Link } from "react-router-dom";
import { AlertTriangle } from "lucide-react";

export default function UnauthorizedPage() {
  return (
    <div className="flex flex-col items-center justify-center h-screen text-center p-4">
      <AlertTriangle className="text-red-500 w-12 h-12 mb-4" />
      <h1 className="text-2xl font-bold mb-2">Accès refusé</h1>
      <p className="text-gray-600 mb-4">
        Vous n’avez pas la permission d’accéder à cette page.
      </p>
      <Link to="/dashboard" className="text-blue-600 hover:underline">
        Retour au tableau de bord
      </Link>
    </div>
  );
}
