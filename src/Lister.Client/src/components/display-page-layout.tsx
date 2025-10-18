import * as React from "react";

import { Stack } from "@mui/material";

const DisplayPageLayout = ({ children }: { children: React.ReactNode }) => {
  return (
    <Stack
      spacing={{ xs: 6, md: 7 }}
      sx={{
        width: "100%",
        maxWidth: 1400,
        mx: "auto",
        px: { xs: 3, md: 8 },
        py: { xs: 4, md: 6 },
      }}
    >
      {children}
    </Stack>
  );
};

export default DisplayPageLayout;
