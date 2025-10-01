import * as React from "react";

import { Link, Paper, Stack, Typography } from "@mui/material";
import { createFileRoute, useRouter } from "@tanstack/react-router";

import { ForgotPasswordDialog, SignInForm, useSideDrawer } from "../components";

const RouteComponent = () => {
  const router = useRouter();
  const navigate = Route.useNavigate();
  const search = Route.useSearch();
  const { auth } = Route.useRouteContext({ select: ({ auth }) => ({ auth }) });
  const status = auth.status;
  const { openDrawer } = useSideDrawer();

  // Removed layout-effect push; navigate after login below handles redirect

  const handleForgotPasswordClick = () => {
    openDrawer("Reset Password", <ForgotPasswordDialog />);
  };

  const handleSignedIn = async (email: string) => {
    auth.login(email);
    await router.invalidate();
    await navigate({ to: search.callbackUrl || "/" });
  };

  return (
    <Stack
      sx={{ width: "450px", mx: "auto", px: 2, pt: 18, pb: 4 }}
      spacing={4}
    >
      <Paper
        elevation={1}
        sx={{
          p: 4,
          borderRadius: 2,
          transition: "box-shadow 0.2s ease-in-out",
          "&:hover": {
            elevation: 2,
          },
        }}
      >
        <Stack spacing={4}>
          <Typography variant="h5" align="center" gutterBottom>
            Sign in
          </Typography>

          <SignInForm onSignedIn={handleSignedIn} />

          <Stack spacing={2} alignItems="center">
            <Link
              component="button"
              type="button"
              onClick={handleForgotPasswordClick}
              sx={{
                textDecoration: "none",
                transition: "color 0.2s ease-in-out",
                "&:hover": {
                  textDecoration: "underline",
                },
              }}
            >
              Forgot your password?
            </Link>

            <Typography
              variant="body2"
              color="text.secondary"
              align="center"
              component="div"
            >
              Don&apos;t have an account?{" "}
              <Link
                component="button"
                type="button"
                onClick={() => navigate({ to: "/sign-up", search })}
                sx={{
                  textDecoration: "none",
                  transition: "color 0.2s ease-in-out",
                  display: "inline",
                  "&:hover": {
                    textDecoration: "underline",
                  },
                }}
              >
                Sign up
              </Link>
            </Typography>
          </Stack>
        </Stack>
      </Paper>
    </Stack>
  );
};

export interface SignInSearch {
  callbackUrl?: string;
}

export const Route = createFileRoute("/sign-in")({
  component: RouteComponent,
  validateSearch: (search): SignInSearch => {
    return {
      callbackUrl: search.callbackUrl as string | undefined,
    };
  },
});
