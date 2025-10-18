import * as React from "react";

import { Link, Stack, Typography } from "@mui/material";
import { useQueryClient } from "@tanstack/react-query";
import { useNavigate, useSearchParams } from "react-router-dom";

import { useAuth } from "../auth";
import { AuthPageLayout, SignUpForm } from "../components";

const SignUpPage = () => {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const queryClient = useQueryClient();
  const auth = useAuth();

  const handleSignedUp = async (email: string) => {
    auth.login(email);
    await queryClient.invalidateQueries();
    const callbackUrl = searchParams.get("callbackUrl") ?? "/";
    navigate(callbackUrl, { replace: true });
  };

  return (
    <AuthPageLayout>
      <Typography variant="h5" align="center" gutterBottom>
        Sign up
      </Typography>

      <SignUpForm onSignedUp={handleSignedUp} />

      <Stack spacing={2}>
        <Typography
          variant="body2"
          color="text.secondary"
          align="center"
          component="div"
        >
          Already have an account?{" "}
          <Link
            component="button"
            type="button"
            onClick={() => {
              const searchString = searchParams.toString();
              navigate(searchString ? `/sign-in?${searchString}` : "/sign-in");
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
            Sign in
          </Link>
        </Typography>
      </Stack>
    </AuthPageLayout>
  );
};
export default SignUpPage;
