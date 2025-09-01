import * as React from "react";

import {
  List as ListIcon,
  Add as AddIcon,
  Menu as MenuIcon,
  AccountCircle as AccountCircleIcon,
} from "@mui/icons-material";
import {
  Avatar,
  Box,
  Drawer,
  IconButton,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Typography,
  useMediaQuery,
  useTheme,
  Chip,
  Divider,
  alpha,
  Menu,
  MenuItem,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  Button,
  AppBar,
  Toolbar,
} from "@mui/material";
import { useNavigate, useRouter } from "@tanstack/react-router";

import { Auth } from "../../auth";

const DRAWER_WIDTH = 280;

interface Props {
  children: React.ReactNode;
  auth: Auth;
  status: string;
}

const AppLayout = ({ children, auth, status }: Props) => {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down("lg"));
  const navigate = useNavigate();
  const router = useRouter();

  const [mobileOpen, setMobileOpen] = React.useState(false);
  const [userMenuAnchor, setUserMenuAnchor] =
    React.useState<null | HTMLElement>(null);
  const [logoutDialogOpen, setLogoutDialogOpen] = React.useState(false);

  const handleDrawerToggle = () => {
    setMobileOpen(!mobileOpen);
  };

  const handleUserMenuClick = (event: React.MouseEvent<HTMLElement>) => {
    setUserMenuAnchor(event.currentTarget);
  };

  const handleUserMenuClose = () => {
    setUserMenuAnchor(null);
  };

  const handleLogoutClick = () => {
    setUserMenuAnchor(null);
    setLogoutDialogOpen(true);
  };

  const handleLogoutConfirm = async () => {
    const request = new Request("/identity/logout", {
      method: "POST",
    });
    const response = await fetch(request);
    if (response.ok) {
      auth.logout();
      router.invalidate();
    }
    setLogoutDialogOpen(false);
  };

  const handleLogoutCancel = () => {
    setLogoutDialogOpen(false);
  };

  const navigationItems = [
    {
      text: "My Lists",
      icon: <ListIcon />,
      path: "/",
      badge: null,
    },
    {
      text: "Create List",
      icon: <AddIcon />,
      path: "/create",
      badge: "New",
    },
  ];

  const drawer = (
    <Box sx={{ height: "100%", display: "flex", flexDirection: "column" }}>
      {/* Logo/Brand Section */}
      <Box
        sx={{
          p: 3,
          borderBottom: {
            xs: "none",
            lg: `1px solid ${theme.palette.divider}`,
          },
        }}
      >
        <Typography variant="h5" fontWeight="bold" color="primary">
          Lister
        </Typography>
      </Box>

      {/* Navigation Items */}
      <Box sx={{ flex: 1, p: 2 }}>
        <List disablePadding>
          {navigationItems.map((item) => (
            <ListItem key={item.text} disablePadding sx={{ mb: 0.5 }}>
              <ListItemButton
                onClick={() => navigate({ to: item.path })}
                sx={{
                  borderRadius: 1,
                  minHeight: 44,
                  "&:hover": {
                    backgroundColor: `${theme.palette.primary.main}08`,
                    transform: "translateX(4px)",
                  },
                  transition: theme.transitions.create(
                    ["background-color", "transform"],
                    {
                      duration: theme.transitions.duration.short,
                    },
                  ),
                  "&.Mui-selected": {
                    backgroundColor: `${theme.palette.primary.main}12`,
                    color: theme.palette.primary.main,
                    "& .MuiListItemIcon-root": {
                      color: theme.palette.primary.main,
                    },
                  },
                }}
              >
                <ListItemIcon sx={{ minWidth: 40 }}>{item.icon}</ListItemIcon>
                <ListItemText
                  primary={item.text}
                  primaryTypographyProps={{
                    fontSize: "0.875rem",
                    fontWeight: 500,
                  }}
                />
                {item.badge && (
                  <Chip
                    label={item.badge}
                    size="small"
                    color="secondary"
                    sx={{
                      height: 20,
                      fontSize: "0.6875rem",
                      fontWeight: 600,
                    }}
                  />
                )}
              </ListItemButton>
            </ListItem>
          ))}
        </List>
      </Box>

      <Divider />

      {/* User Profile Section */}
      <Box sx={{ p: 2 }}>
        <ListItemButton
          onClick={handleUserMenuClick}
          sx={{
            borderRadius: 1,
            p: 2,
            "&:hover": {
              backgroundColor: alpha(theme.palette.primary.main, 0.08),
              transform: "translateY(-1px)",
              boxShadow: theme.shadows[2],
            },
            transition: theme.transitions.create(
              ["background-color", "transform", "box-shadow"],
              {
                duration: theme.transitions.duration.short,
              },
            ),
          }}
        >
          <Avatar sx={{ width: 36, height: 36, mr: 2 }}>
            <AccountCircleIcon />
          </Avatar>
          <Box sx={{ flex: 1, minWidth: 0 }}>
            <Typography variant="subtitle2" noWrap>
              {auth.username || "User Account"}
            </Typography>
            <Typography variant="caption" color="text.secondary">
              Logged in
            </Typography>
          </Box>
        </ListItemButton>
      </Box>
    </Box>
  );

  if (status !== "loggedIn") {
    return <>{children}</>;
  }

  return (
    <Box sx={{ display: "flex", minHeight: "100vh" }}>
      {/* Mobile Header */}
      {isMobile && (
        <AppBar
          position="fixed"
          sx={{
            zIndex: theme.zIndex.drawer + 1,
            backgroundColor: theme.palette.background.paper,
            color: theme.palette.text.primary,
            boxShadow: theme.shadows[1],
            borderBottom: `1px solid ${theme.palette.divider}`,
          }}
        >
          <Toolbar>
            <IconButton
              edge="start"
              onClick={handleDrawerToggle}
              sx={{ ml: 1, mr: 2 }}
            >
              <MenuIcon />
            </IconButton>
            <Typography variant="h6" fontWeight="bold" color="primary">
              Lister
            </Typography>
          </Toolbar>
        </AppBar>
      )}

      {/* Sidebar Navigation */}
      <Box
        component="nav"
        sx={{ width: { lg: DRAWER_WIDTH }, flexShrink: { lg: 0 } }}
      >
        {/* Mobile drawer */}
        <Drawer
          variant="temporary"
          open={mobileOpen}
          onClose={handleDrawerToggle}
          ModalProps={{
            keepMounted: true,
          }}
          sx={{
            display: { xs: "block", lg: "none" },
            "& .MuiDrawer-paper": {
              boxSizing: "border-box",
              width: DRAWER_WIDTH,
              borderRight: `1px solid ${theme.palette.divider}`,
            },
          }}
        >
          {drawer}
        </Drawer>

        {/* Desktop drawer */}
        <Drawer
          variant="permanent"
          sx={{
            display: { xs: "none", lg: "block" },
            "& .MuiDrawer-paper": {
              boxSizing: "border-box",
              width: DRAWER_WIDTH,
              borderRight: `1px solid ${theme.palette.divider}`,
            },
          }}
          open
        >
          {drawer}
        </Drawer>
      </Box>

      {/* Main content */}
      <Box
        component="main"
        sx={{
          flexGrow: 1,
          width: { lg: `calc(100% - ${DRAWER_WIDTH}px)` },
          minHeight: "100vh",
          backgroundColor: theme.palette.background.default,
        }}
      >
        <Box
          sx={{
            p: { xs: 2, lg: 3 },
            mt: { xs: 13, sm: 15, lg: 0 },
          }}
        >
          {children}
        </Box>
      </Box>

      {/* User Menu */}
      <Menu
        anchorEl={userMenuAnchor}
        open={Boolean(userMenuAnchor)}
        onClose={handleUserMenuClose}
        anchorOrigin={{
          vertical: "top",
          horizontal: "right",
        }}
        transformOrigin={{
          vertical: "bottom",
          horizontal: "right",
        }}
      >
        <MenuItem onClick={handleLogoutClick}>
          <Typography color="error.main">Log Out</Typography>
        </MenuItem>
      </Menu>

      {/* Logout Confirmation Dialog */}
      <Dialog open={logoutDialogOpen} onClose={handleLogoutCancel}>
        <DialogTitle>Confirm Logout</DialogTitle>
        <DialogContent>
          <DialogContentText>
            Are you sure you want to log out?
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleLogoutCancel}>Cancel</Button>
          <Button
            onClick={handleLogoutConfirm}
            color="error"
            variant="contained"
          >
            Log Out
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default AppLayout;
