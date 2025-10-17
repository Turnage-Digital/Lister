import * as React from "react";

import { Link, Paper, Stack, Typography } from "@mui/material";
import { useQueryClient } from "@tanstack/react-query";
import { useNavigate, useSearchParams } from "react-router-dom";

import { useAuth } from "../auth";
import { SignInForm } from "../components";

const SignInPage = () => {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const queryClient = useQueryClient();
  const auth = useAuth();

  const handleForgotPasswordClick = () => {
    const query = searchParams.toString();
    navigate(query ? `/forgot-password?${query}` : "/forgot-password");
  };

  const handleSignedIn = async (email: string) => {
    auth.login(email);
    await queryClient.invalidateQueries();
    const callbackUrl = searchParams.get("callbackUrl") ?? "/";
    navigate(callbackUrl, { replace: true });
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
                onClick={() => {
                  const searchString = searchParams.toString();
                  navigate(
                    searchString ? `/sign-up?${searchString}` : "/sign-up",
                  );
                }}
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
export default SignInPage;
