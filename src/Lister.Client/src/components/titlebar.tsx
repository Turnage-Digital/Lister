import * as React from "react";

import {
  Box,
  Breadcrumbs,
  Button,
  Grid,
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
      <Grid size={{ xs: 12, md: 9 }}>
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
        <Grid
          size={{ xs: 12, md: 3 }}
          sx={{
            display: { xs: "none", md: "flex" },
            justifyContent: "flex-end",
          }}
        >
          <Stack direction="row" spacing={2}>
            {actions.map((action) => (
              <Box key={action.title}>
                <Button
                  variant="contained"
                  startIcon={action.icon}
                  onClick={action.onClick}
                >
                  {action.title}
                </Button>
              </Box>
            ))}
          </Stack>
        </Grid>
      )}

      {breadcrumbs && breadcrumbs.length > 0 && (
        <Grid size={{ xs: 12 }}>
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
