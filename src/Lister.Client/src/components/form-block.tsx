import { Grid2, Stack, Typography } from "@mui/material";
import React, { ReactNode } from "react";

interface Props {
  title: string;
  blurb: string;
  content: ReactNode;
}

const FormBlock = ({ title, blurb, content }: Props) => {
  return (
    <Grid2 container>
      <Grid2 xs={12} md={4}>
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
      </Grid2>
      <Grid2 xs={12} md={8}>
        {content}
      </Grid2>
    </Grid2>
  );
};

export default FormBlock;
