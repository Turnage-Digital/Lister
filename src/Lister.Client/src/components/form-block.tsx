import * as React from "react";
import { ReactNode } from "react";

import { Grid, Stack, Typography } from "@mui/material";

interface Props {
  title: string;
  subtitle?: string;
  content: ReactNode;
}

const FormBlock = ({ title, subtitle, content }: Props) => {
  return (
    <Grid container>
      <Grid size={{ xs: 12, md: 4 }}>
        <Stack spacing={1} sx={{ pb: { xs: 4, md: 0 } }}>
          <Typography color="primary" fontWeight="medium" variant="h6">
            {title}
          </Typography>
          {subtitle && (
            <Typography variant="body2" color="text.secondary">
              {subtitle}
            </Typography>
          )}
        </Stack>
      </Grid>
      <Grid size={{ xs: 12, md: 4 }}>{content}</Grid>
    </Grid>
  );
};

export default FormBlock;
