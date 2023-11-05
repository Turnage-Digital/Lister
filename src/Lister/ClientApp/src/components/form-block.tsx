import React, { ReactNode } from "react";
import { Stack, Typography } from "@mui/material";
import Grid from "@mui/material/Unstable_Grid2";

interface Props {
  title: string;
  blurb: string;
  content: ReactNode;
}

const FormBlock = ({ title, blurb, content }: Props) => {
  return (
    <Grid container spacing={2}>
      <Grid xs={12} md={4}>
        <Stack spacing={2}>
          <Typography variant="h6" fontWeight="bold">
            {title}
          </Typography>
          <Typography variant="body1" sx={{ color: "text.secondary" }}>
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
