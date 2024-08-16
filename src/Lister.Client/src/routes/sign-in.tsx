import React, { FormEvent } from "react";
import {
  Box,
  Button,
  Container,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { createFileRoute, redirect } from "@tanstack/react-router";

const SignIn = () => {
  // const actionData = useActionData() as { error: string } | undefined;

  const searchParams = Route.useSearch();
  const redirectTo = searchParams.callbackUrl;

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const formData = new FormData(event.currentTarget);

    const email = formData.get("username") as string | null;
    if (!email) {
      return {
        error: "You must provide a username to log in",
      };
    }

    const password = formData.get("password") as string | null;
    if (!password) {
      return {
        error: "You must provide a password to log in",
      };
    }

    const input = { email, password };
    const postRequest = new Request("/identity/login?useCookies=true", {
      headers: {
        "Content-Type": "application/json",
      },
      method: "POST",
      body: JSON.stringify(input),
    });
    const response = await fetch(postRequest);
    const succeeded = response.status === 200;
    if (!succeeded) {
      return {
        error: "Invalid username or password",
      };
    }

    // const redirectTo = formData.get("redirectTo") as string | null;
    // return redirect({ to: redirectTo || "/" });
  };

  return (
    <Container component="main" maxWidth="xs" sx={{ mt: 8 }}>
      <Stack spacing={2} alignItems="center">
        <Typography variant="h5">Sign in</Typography>

        <Box component="form" method="post">
          <input type="hidden" name="redirectTo" value={redirectTo} />

          <TextField
            margin="normal"
            required
            fullWidth
            name="username"
            id="username"
            label="Username"
            autoComplete="username"
          />

          <TextField
            margin="normal"
            required
            fullWidth
            name="password"
            id="password"
            label="Password"
            type="password"
            autoComplete="current-password"
          />

          <Button
            type="submit"
            variant="contained"
            size="large"
            fullWidth
            sx={{ my: 2 }}
          >
            Sign In
          </Button>
        </Box>

        {/* {actionData && actionData.error && (*/}
        {/*  <Alert sx={{ mt: 2 }} severity="error">*/}
        {/*    {actionData.error}*/}
        {/*  </Alert>*/}
        {/* )}*/}
      </Stack>
    </Container>
  );
};

export interface SignInSearch {
  callbackUrl: string;
}

export const Route = createFileRoute("/sign-in")({
  component: SignIn,
  validateSearch: (search): SignInSearch => {
    return {
      callbackUrl: search?.callback as string,
    };
  },
});
