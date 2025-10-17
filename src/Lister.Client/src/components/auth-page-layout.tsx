import * as React from "react";

import { Paper, Stack } from "@mui/material";

interface AuthPageLayoutProps {
  children: React.ReactNode;
}

const AuthPageLayout = ({ children }: AuthPageLayoutProps) => (
  <Stack
    sx={{
      maxWidth: 450,
      width: "100%",
      mx: "auto",
      px: { xs: 2, md: 2 },
      py: { xs: 4, md: 4 },
      pt: 18,
    }}
    spacing={{ xs: 4, md: 4 }}
  >
    <Paper
      elevation={1}
      sx={{
        p: 4,
        borderRadius: 2,
        transition: "box-shadow 0.2s ease-in-out",
        "&:hover": {
          elevation: 2,
        },
      }}
    >
      <Stack spacing={4}>{children}</Stack>
    </Paper>
  </Stack>
);

export default AuthPageLayout;
