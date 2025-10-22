import * as React from "react";

import { Stack } from "@mui/material";

const defaultSpacing = { xs: 6, md: 7 } as const;

const EditorPageLayout = ({ children }: { children: React.ReactNode }) => {
  return (
    <Stack
      spacing={defaultSpacing}
      sx={{
        width: "100%",
        maxWidth: 1180,
        mx: "auto",
        px: { xs: 3, md: 7 },
        py: { xs: 4, md: 6 },
      }}
    >
      {children}
    </Stack>
  );
};

export default EditorPageLayout;
