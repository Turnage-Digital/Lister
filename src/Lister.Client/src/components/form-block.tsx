import * as React from "react";
import { ReactNode } from "react";

import { Grid, Stack, Typography } from "@mui/material";

interface Props {
  title: string;
  blurb: string;
  content: ReactNode;
}

const FormBlock = ({ title, blurb, content }: Props) => {
  return (
    <Grid container>
      <Grid size={{ xs: 12, md: 4 }}>
        <Stack spacing={2} sx={{ pb: { xs: 4, md: 0 } }}>
          <Typography color="primary" fontWeight="medium" variant="h6">
            {title}
          </Typography>
          <Typography
            variant="body1"
            sx={{ color: "text.secondary" }}
            gutterBottom
          >
            {blurb}
          </Typography>
        </Stack>
      </Grid>
      <Grid size={{ xs: 12, md: 4 }}>{content}</Grid>
    </Grid>
  );
};

export default FormBlock;
