import React from "react";
import { Box } from "@mui/material";

import { ContentSection, TopSection } from "./layout";

const Shell = () => {
  return (
    <Box sx={{ display: "flex" }}>
      <TopSection />
      <ContentSection />
    </Box>
  );
};

export default Shell;
