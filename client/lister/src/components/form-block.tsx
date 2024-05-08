import { Stack, Typography } from "@mui/material";
import Grid from "@mui/material/Unstable_Grid2";
import React, { ReactNode } from "react";

interface Props {
  title: string;
  blurb: string;
  content: ReactNode;
}

const FormBlock = ({ title, blurb, content }: Props) => {
  return (
    <Grid container>
      <Grid xs={12} md={4}>
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
      <Grid xs={12} md={8}>
        {content}
      </Grid>
    </Grid>
  );
};

export default FormBlock;
