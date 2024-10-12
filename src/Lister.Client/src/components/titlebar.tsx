import {
  Box,
  Breadcrumbs,
  Button,
  Grid2,
  Hidden,
  Link,
  Stack,
  Typography,
} from "@mui/material";
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
    <Grid2 container>
      <Grid2 size={{ xs: 12, md: 9 }}>
        <Typography
          color="primary"
          fontWeight="medium"
          variant="h4"
          component="h1"
          gutterBottom
        >
          {title}
        </Typography>
      </Grid2>

      {actions && actions.length > 0 && (
        <Box sx={{ display: { xs: "none", md: "flex" } }}>
          <Grid2
            size={{ xs: 12, md: 3 }}
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
          </Grid2>
        </Box>
      )}

      {breadcrumbs && breadcrumbs.length > 0 && (
        <Grid2 size={{ xs: 12 }}>
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
        </Grid2>
      )}
    </Grid2>
  );
};

export default Titlebar;
