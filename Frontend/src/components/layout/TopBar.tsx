
import { Bell } from "lucide-react";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Button } from "@/components/ui/button";
import { SidebarTrigger } from "@/components/ui/sidebar";
import { useAuth } from "@/hooks/useAuth";
import { useState } from "react";
import { 
  DropdownMenu, 
  DropdownMenuContent, 
  DropdownMenuTrigger 
} from "@/components/ui/dropdown-menu";
import { NotificationDropdown } from "@/components/notifications/NotificationDropdown";
import { useNavigate } from "react-router-dom";
import { ThemeSwitcher } from "@/components/ui/theme-switcher";
import { LanguageSwitcher } from "@/components/ui/language-switcher";
import { 
  HoverCard, 
  HoverCardContent, 
  HoverCardTrigger 
} from "@/components/ui/hover-card";

export function TopBar() {
  const { user } = useAuth();
  const navigate = useNavigate();
  const [notificationsOpen, setNotificationsOpen] = useState(false);
  
  const getInitials = (name: string) => {
    return name
      .split(' ')
      .map(n => n[0])
      .join('')
      .toUpperCase();
  };

  const handleProfileClick = () => {
    if (user?.role === 'Patient') {
      navigate('/patients');
    } else if (user?.role === 'Doctor') {
      navigate('/doctors');
    } else {
      navigate('/profile');
    }
  };
  
  return (
    <header className="fixed top-0 left-0 right-0 h-16 border-b border-border bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60 z-10 flex items-center justify-between px-4 lg:px-6">
      <div className="flex items-center gap-2">
        <SidebarTrigger className="h-9 w-9" />
      </div>
      
      <div className="flex items-center gap-4">
        <LanguageSwitcher />
        <ThemeSwitcher />
        
        <DropdownMenu 
          open={notificationsOpen} 
          onOpenChange={setNotificationsOpen}
        >
          <DropdownMenuTrigger asChild>
            <Button variant="ghost" size="icon" className="relative">
              <Bell className="h-5 w-5" />
              <span className="absolute top-1 right-1.5 w-2 h-2 bg-red-500 rounded-full"></span>
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end" className="w-80 p-0">
            <NotificationDropdown onClose={() => setNotificationsOpen(false)} />
          </DropdownMenuContent>
        </DropdownMenu>
        
        {user && (
          <HoverCard>
            <HoverCardTrigger asChild>
              <Button 
                variant="ghost" 
                className="flex items-center gap-3 p-0 px-2 hover:bg-primary/10 transition-colors"
                onClick={handleProfileClick}
              >
                <div className="text-right">
                  <p className="text-sm font-medium">{user.name}</p>
                  <p className="text-xs text-muted-foreground">{user.role}</p>
                </div>
                <Avatar>
                  <AvatarImage src="" />
                  <AvatarFallback className="bg-clinic-500 text-white">
                    {getInitials(user.name)}
                  </AvatarFallback>
                </Avatar>
              </Button>
            </HoverCardTrigger>
            <HoverCardContent className="p-3">
              <div className="space-y-2">
                <p className="font-medium">{user.name}</p>
                <p className="text-sm text-muted-foreground">{user.role}</p>
                <Button 
                  onClick={handleProfileClick} 
                  variant="outline" 
                  size="sm"
                  className="w-full mt-2"
                >
                  View profile
                </Button>
              </div>
            </HoverCardContent>
          </HoverCard>
        )}
      </div>
    </header>
  );
}
