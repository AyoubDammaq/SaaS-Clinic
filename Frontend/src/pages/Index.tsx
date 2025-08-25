import { useEffect } from "react";
import { useNavigate, Link } from "react-router-dom";
import { useAuth } from "@/hooks/useAuth";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { useTranslation } from "@/hooks/useTranslation";
import {
  Calendar,
  Users,
  FileText,
  CreditCard,
  Shield,
  Clock,
  Activity,
  Smartphone,
  BarChart3,
  Stethoscope,
  UserCheck,
  Building2,
} from "lucide-react";

const Index = () => {
  const navigate = useNavigate();
  const { isAuthenticated } = useAuth();
  const { t } = useTranslation();

  useEffect(() => {
    // If authenticated, go to dashboard
    if (isAuthenticated) {
      navigate("/dashboard");
    }
  }, [isAuthenticated, navigate]);

  const features = [
    {
      icon: Calendar,
      title: "Gestion des Rendez-vous",
      description:
        "Planifiez et gérez facilement tous vos rendez-vous médicaux avec un système de calendrier intuitif.",
    },
    {
      icon: Users,
      title: "Gestion des Patients",
      description:
        "Centralisez toutes les informations de vos patients dans un dossier médical sécurisé et accessible.",
    },
    {
      icon: Stethoscope,
      title: "Consultations Médicales",
      description:
        "Documentez et suivez toutes vos consultations avec des outils adaptés aux professionnels de santé.",
    },
    {
      icon: UserCheck,
      title: "Gestion des Médecins",
      description:
        "Organisez votre équipe médicale, leurs spécialités et leurs disponibilités en toute simplicité.",
    },
    {
      icon: Building2,
      title: "Multi-Cliniques",
      description:
        "Gérez plusieurs établissements de santé depuis une seule plateforme centralisée.",
    },
    {
      icon: CreditCard,
      title: "Facturation Intégrée",
      description:
        "Automatisez votre facturation et suivez vos paiements avec un système comptable intégré.",
    },
    {
      icon: FileText,
      title: "Dossiers Médicaux",
      description:
        "Accédez rapidement aux antécédents médicaux, prescriptions et résultats d'examens.",
    },
    {
      icon: BarChart3,
      title: "Analyses & Rapports",
      description:
        "Obtenez des insights précieux sur votre activité avec des tableaux de bord personnalisés.",
    },
    {
      icon: Shield,
      title: "Sécurité & Conformité",
      description:
        "Respectez les normes RGPD et de confidentialité médicale avec notre infrastructure sécurisée.",
    },
  ];

  const benefits = [
    {
      icon: Clock,
      title: "Gain de Temps",
      description:
        "Automatisez vos tâches administratives et concentrez-vous sur vos patients.",
    },
    {
      icon: Activity,
      title: "Efficacité Accrue",
      description: "Optimisez vos processus et améliorez la qualité des soins.",
    },
    {
      icon: Smartphone,
      title: "Accessible Partout",
      description:
        "Accédez à votre plateforme depuis n'importe quel appareil, à tout moment.",
    },
  ];

  return (
    <div className="min-h-screen bg-gradient-to-br from-background via-muted/20 to-background">
      {/* Hero Section */}
      <section className="relative overflow-hidden">
        <div className="container mx-auto px-4 py-16 lg:py-24">
          <div className="text-center max-w-4xl mx-auto">
            <div className="w-20 h-20 bg-clinic-500 text-white rounded-2xl flex items-center justify-center font-bold text-3xl mx-auto mb-8 shadow-lg">
              SC
            </div>
            <h1 className="text-5xl lg:text-7xl font-bold mb-6 bg-gradient-to-r from-clinic-600 to-clinic-400 bg-clip-text text-transparent">
              SaaS-Clinic
            </h1>
            <p className="text-xl lg:text-2xl text-muted-foreground mb-8 leading-relaxed">
              La plateforme moderne de gestion médicale pour <br />
              <span className="text-clinic-600 font-semibold">
                cliniques
              </span>{" "}
              et{" "}
              <span className="text-clinic-600 font-semibold">
                professionnels de santé
              </span>
            </p>
            <p className="text-lg text-muted-foreground mb-12 max-w-2xl mx-auto">
              Simplifiez votre quotidien médical avec une solution complète qui
              centralise la gestion des patients, rendez-vous, consultations et
              bien plus encore.
            </p>

            <div className="flex flex-col sm:flex-row gap-4 justify-center">
              <Button asChild size="lg" className="text-lg px-8 py-6">
                <Link to="/register">Commencer Gratuitement</Link>
              </Button>
              <Button
                asChild
                variant="outline"
                size="lg"
                className="text-lg px-8 py-6"
              >
                <Link to="/login">Se Connecter</Link>
              </Button>
            </div>
          </div>
        </div>
      </section>

      {/* Features Section */}
      <section className="py-16 lg:py-24 bg-muted/30">
        <div className="container mx-auto px-4">
          <div className="text-center mb-16">
            <h2 className="text-4xl lg:text-5xl font-bold mb-6">
              Fonctionnalités <span className="text-clinic-600">Complètes</span>
            </h2>
            <p className="text-xl text-muted-foreground max-w-3xl mx-auto">
              Découvrez tous les outils dont vous avez besoin pour gérer
              efficacement votre pratique médicale au quotidien.
            </p>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
            {features.map((feature, index) => (
              <Card
                key={index}
                className="group hover:shadow-lg transition-all duration-300 border-0 bg-background/80 backdrop-blur"
              >
                <CardHeader className="pb-4">
                  <div className="w-14 h-14 bg-clinic-100 rounded-xl flex items-center justify-center mb-4 group-hover:bg-clinic-500 group-hover:text-white transition-colors">
                    <feature.icon className="w-7 h-7 text-clinic-600 group-hover:text-white" />
                  </div>
                  <CardTitle className="text-xl group-hover:text-clinic-600 transition-colors">
                    {feature.title}
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  <CardDescription className="text-base leading-relaxed">
                    {feature.description}
                  </CardDescription>
                </CardContent>
              </Card>
            ))}
          </div>
        </div>
      </section>

      {/* Benefits Section */}
      <section className="py-16 lg:py-24">
        <div className="container mx-auto px-4">
          <div className="text-center mb-16">
            <h2 className="text-4xl lg:text-5xl font-bold mb-6">
              Pourquoi Choisir{" "}
              <span className="text-clinic-600">SaaS-Clinic</span> ?
            </h2>
            <p className="text-xl text-muted-foreground max-w-3xl mx-auto">
              Rejoignez des milliers de professionnels de santé qui font
              confiance à notre plateforme pour optimiser leur pratique.
            </p>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-8 max-w-5xl mx-auto">
            {benefits.map((benefit, index) => (
              <div key={index} className="text-center group">
                <div className="w-20 h-20 bg-gradient-to-br from-clinic-500 to-clinic-600 rounded-2xl flex items-center justify-center mb-6 mx-auto shadow-lg group-hover:scale-110 transition-transform">
                  <benefit.icon className="w-10 h-10 text-white" />
                </div>
                <h3 className="text-2xl font-bold mb-4 group-hover:text-clinic-600 transition-colors">
                  {benefit.title}
                </h3>
                <p className="text-muted-foreground text-lg leading-relaxed">
                  {benefit.description}
                </p>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* CTA Section */}
      <section className="py-16 lg:py-24 bg-gradient-to-r from-clinic-600 to-clinic-500">
        <div className="container mx-auto px-4 text-center">
          <div className="max-w-4xl mx-auto text-white">
            <h2 className="text-4xl lg:text-5xl font-bold mb-6">
              Prêt à Révolutionner Votre Pratique ?
            </h2>
            <p className="text-xl mb-8 opacity-90 leading-relaxed">
              Rejoignez SaaS-Clinic dès aujourd'hui et découvrez comment notre
              plateforme peut transformer votre gestion médicale quotidienne.
            </p>
            <div className="flex flex-col sm:flex-row gap-4 justify-center">
              <Button
                asChild
                size="lg"
                variant="secondary"
                className="text-lg px-8 py-6"
              >
                <Link to="/register">Créer Mon Compte</Link>
              </Button>
              <Button
                asChild
                size="lg"
                variant="outline"
                className="text-lg px-8 py-6 border-white text-white hover:bg-white hover:text-clinic-600"
              >
                <Link to="/login">J'ai Déjà Un Compte</Link>
              </Button>
            </div>
          </div>
        </div>
      </section>

      {/* Footer */}
      <footer className="py-12 bg-muted/50 border-t">
        <div className="container mx-auto px-4 text-center">
          <div className="w-12 h-12 bg-clinic-500 text-white rounded-lg flex items-center justify-center font-bold text-xl mx-auto mb-4">
            SC
          </div>
          <p className="text-muted-foreground mb-4">
            SaaS-Clinic - La solution moderne pour les professionnels de santé
          </p>
          <p className="text-sm text-muted-foreground">
            © 2024 SaaS-Clinic. Tous droits réservés.
          </p>
        </div>
      </footer>
    </div>
  );
};

export default Index;
