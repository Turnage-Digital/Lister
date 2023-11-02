import React from "react";
import {
  Box,
  Drawer,
  Hidden,
  List,
  ListItemButton,
  ListItemIcon,
  ListItemText,
} from "@mui/material";
import { Home, RocketLaunch } from "@mui/icons-material";

interface Props {
  drawerOpen: boolean;
  onDrawerToggle: (open: boolean) => void;
  selectedRoute: string;
  onRouteSelected: (route: string) => void;
}

const SideSection = ({
  drawerOpen,
  onDrawerToggle,
  selectedRoute,
  onRouteSelected,
}: Props) => {
  const width: 240 = 240;
  const drawer = (
    <>
      <Box sx={(theme) => ({ width, ...theme.mixins.toolbar })} />
      <List sx={{ width }}>
        <ListItemButton
          selected={selectedRoute === "/"}
          onClick={() => onRouteSelected("/")}
        >
          <ListItemIcon>
            <Home />
          </ListItemIcon>
          <ListItemText primary="Home" />
        </ListItemButton>

        <ListItemButton
          selected={selectedRoute.includes("/thing-defs")}
          onClick={() => onRouteSelected("/thing-defs")}
        >
          <ListItemIcon>
            <RocketLaunch />
          </ListItemIcon>
          <ListItemText primary="Thing Definitions" />
        </ListItemButton>
      </List>
    </>
  );

  return (
    <>
      <Hidden mdDown>
        <Drawer variant="permanent" anchor="left" sx={{ width }}>
          {drawer}
        </Drawer>
      </Hidden>

      <Hidden mdUp>
        <Drawer
          variant="temporary"
          anchor="left"
          open={drawerOpen}
          onClose={onDrawerToggle}
          sx={{ width }}
        >
          {drawer}
        </Drawer>
      </Hidden>
    </>
  );
};

export default SideSection;
