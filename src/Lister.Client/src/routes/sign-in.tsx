import React, { FormEvent, useState } from "react";
import {
  Alert,
  Box,
  Button,
  Container,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { createFileRoute } from "@tanstack/react-router";

const RouteComponent = () => {
  const navigate = Route.useNavigate();
  const search = Route.useSearch();

  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    const formData = new FormData(e.currentTarget);

    const email = formData.get("username") as string | null;
    if (!email) {
      setError("You must provide a username to log in");
      return;
    }

    const password = formData.get("password") as string | null;
    if (!password) {
      setError("You must provide a password to log in");
      return;
    }

    const input = { email, password };
    const request = new Request("/identity/login?useCookies=true", {
      headers: {
        "Content-Type": "application/json",
      },
      method: "POST",
      body: JSON.stringify(input),
    });
    const response = await fetch(request);
    if (!response.ok) {
      setError("Invalid username or password");
      return;
    }

    navigate({ to: search.callbackUrl });
  };

  return (
    <Container component="main" maxWidth="xs" sx={{ mt: 8 }}>
      <Stack spacing={2} alignItems="center">
        <Typography variant="h5">Sign in</Typography>

        <Box component="form" method="post" onSubmit={handleSubmit}>
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

        {error && (
          <Alert severity="error" sx={{ width: "100%" }}>
            {error}
          </Alert>
        )}
      </Stack>
    </Container>
  );
};

export interface SignInSearch {
  callbackUrl?: string;
}

export const Route = createFileRoute("/sign-in")({
  component: RouteComponent,
  validateSearch: (search): SignInSearch => {
    return {
      callbackUrl: search?.callbackUrl as string | undefined,
    };
  },
});