import React from "react";
import Grid from "@mui/material/Unstable_Grid2";
import { Button, Hidden, Stack, Typography } from "@mui/material";

interface Action {
  title: string;
  icon: React.ReactNode;
  onClick: () => void;
}

interface Props {
  title: string;
  actions?: Action[];
}

const Titlebar = ({ title, actions }: Props) => {
  return (
    <Grid container sx={{ p: 4 }}>
      <Grid xs={12} md={9}>
        <Typography
          color="primary"
          fontWeight="bold"
          variant="h5"
          component="h1"
        >
          {title}
        </Typography>
      </Grid>

      <Grid xs={12} md={3} display="flex" justifyContent="flex-end">
        <Hidden mdDown>
          <Stack direction="row" spacing={2}>
            {actions?.map((action) => (
              <Button
                key={action.title}
                variant="contained"
                size="small"
                startIcon={action.icon}
                onClick={action.onClick}
              >
                {action.title}
              </Button>
            ))}
          </Stack>
        </Hidden>
      </Grid>
    </Grid>
  );
};

export default Titlebar;
