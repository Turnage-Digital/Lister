import React from "react";
import Grid from "@mui/material/Unstable_Grid2";
import {
  Breadcrumbs,
  Button,
  Hidden,
  Link,
  Stack,
  Typography,
} from "@mui/material";

export interface Action {
  title: string;
  icon?: React.ReactNode;
  onClick?: () => void;
}

export interface Breadcrumb {
  title: string;
  url: string;
}

export interface TitlebarProps {
  title: string;
  actions?: Action[];
  breadcrumbs?: Breadcrumb[];
}

const Titlebar = ({ title, actions, breadcrumbs }: TitlebarProps) => {
  return (
    <Grid container spacing={2} sx={{ p: 4 }}>
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
        <Grid xs={12} md={3} display="flex" justifyContent="flex-end">
          <Hidden mdDown>
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
          </Hidden>
        </Grid>
      )}

      {breadcrumbs && breadcrumbs.length > 0 && (
        <Grid xs={12}>
          <Breadcrumbs>
            {breadcrumbs.map((breadcrumb) => (
              <Link
                key={breadcrumb.url}
                href={breadcrumb.url}
                underline="hover"
                sx={{ cursor: "pointer" }}
              >
                {breadcrumb.title}
              </Link>
            ))}
          </Breadcrumbs>
        </Grid>
      )}
    </Grid>
  );
};

export default Titlebar;
