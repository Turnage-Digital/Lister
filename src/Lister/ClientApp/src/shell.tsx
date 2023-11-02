import { Box } from "@mui/material";
import React, { useCallback, useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";

import { ContentSection, SideSection, TopSection } from "./layout";

const Shell = () => {
  const location = useLocation();
  const navigate = useNavigate();

  const [drawerOpen, setDrawerOpen] = useState(false);
  const [selectedRoute, setSelectedRoute] = useState("");

  const handleRouteSelection = useCallback(
    (route: string) => {
      setSelectedRoute(route);
      setDrawerOpen(false);
      navigate(route);
    },
    [navigate]
  );

  useEffect(
    () => handleRouteSelection(location.pathname),
    [handleRouteSelection, location.pathname]
  );

  const handleDrawerToggle = () => {
    setDrawerOpen(!drawerOpen);
  };

  return (
    <Box sx={{ display: "flex" }}>
      <TopSection onDrawerToggle={handleDrawerToggle} />
      <SideSection
        drawerOpen={drawerOpen}
        onDrawerToggle={handleDrawerToggle}
        selectedRoute={selectedRoute}
        onRouteSelected={handleRouteSelection}
      />
      <ContentSection />
    </Box>
  );
};

export default Shell;
