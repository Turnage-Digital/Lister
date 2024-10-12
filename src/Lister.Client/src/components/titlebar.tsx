import { Breadcrumbs, Button, Hidden, Link, Stack, Typography } from "@mui/material";
import Grid from "@mui/material/Unstable_Grid2";
import React from "react";

export interface Action {
  title: string;
  icon?: React.ReactNode;
  onClick?: () => void;
}

export interface Breadcrumb {
  title: string;
  onClick?: () => void;
}

export interface TitlebarProps {
  title: string;
  actions?: Action[];
  breadcrumbs?: Breadcrumb[];
}

const Titlebar = ({ title, actions, breadcrumbs }: TitlebarProps) => {
  return (
    <Grid container>
      <Grid xs={12} md={9}>
        <Typography
          color="primary"
          fontWeight="medium"
          variant="h4"
          component="h1"
          gutterBottom
        >
          {title}
        </Typography>
      </Grid>

      {actions && actions.length > 0 && (
        <Hidden mdDown>
          <Grid
            xs={12}
            md={3}
            display="flex"
            alignItems="center"
            justifyContent="flex-end"
          >
            <Stack direction="row" spacing={2}>
              {actions.map((action) => (
                <Button
                  key={action.title}
                  variant="contained"
                  startIcon={action.icon}
                  onClick={action.onClick}
                >
                  {action.title}
                </Button>
              ))}
            </Stack>
          </Grid>
        </Hidden>
      )}

      {breadcrumbs && breadcrumbs.length > 0 && (
        <Grid xs={12}>
          <Breadcrumbs separator="›">
            {breadcrumbs.map((breadcrumb) => (
              <Link
                key={breadcrumb.title}
                underline="hover"
                onClick={breadcrumb.onClick}
                sx={{ cursor: "pointer" }}
              >
                {breadcrumb.title}
              </Link>
            ))}
            <Typography color="text.secondary">{title}</Typography>
          </Breadcrumbs>
        </Grid>
      )}
    </Grid>
  );
};

export default Titlebar;
