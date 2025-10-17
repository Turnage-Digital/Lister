import * as React from "react";

import { Stack, type StackProps } from "@mui/material";

type EditorPageLayoutProps = StackProps;

const defaultSpacing = { xs: 6, md: 7 } as const;

const baseSx = {
  maxWidth: 1180,
  width: "100%",
  mx: "auto",
  px: { xs: 3, md: 7 },
  py: { xs: 4, md: 6 },
} as const;

const EditorPageLayout = ({
  children,
  spacing,
  sx,
  ...stackProps
}: EditorPageLayoutProps) => {
  let combinedSx: StackProps["sx"];

  if (Array.isArray(sx)) {
    combinedSx = [baseSx, ...sx] as StackProps["sx"];
  } else if (sx) {
    combinedSx = [baseSx, sx] as StackProps["sx"];
  } else {
    combinedSx = [baseSx];
  }

  return (
    <Stack spacing={spacing ?? defaultSpacing} sx={combinedSx} {...stackProps}>
      {children}
    </Stack>
  );
};

export default EditorPageLayout;
