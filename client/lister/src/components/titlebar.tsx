import React from "react";
import {
  Breadcrumbs,
  Button,
  Hidden,
  Link,
  Stack,
  Typography,
} from "@mui/material";
import Grid from "@mui/material/Unstable_Grid2";

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
    <Grid container spacing={2}>
      <Grid xs={12} md={9}>
        <Typography
          color="primary"
          fontWeight="medium"
          variant="h4"
          component="h1"
        >
          {title}
        </Typography>
      </Grid>

      {actions && actions.length > 0 && (
        <Hidden mdDown>
          <Grid xs={12} md={3} display="flex" justifyContent="flex-end">
            <Stack direction="row" spacing={2}>
              {actions.map((action) => (
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
          </Grid>
        </Hidden>
      )}

      {breadcrumbs && breadcrumbs.length > 0 && (
        <Grid xs={12}>
          <Breadcrumbs>
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
            <Typography color="text.primary">{title}</Typography>
          </Breadcrumbs>
        </Grid>
      )}
    </Grid>
  );
};

export default Titlebar;
