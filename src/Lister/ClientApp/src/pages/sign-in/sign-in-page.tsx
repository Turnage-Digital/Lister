import React from "react";
import {
  Alert,
  Button,
  Container,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { Form, useActionData, useLocation } from "react-router-dom";

const SignInPage = () => {
  const location = useLocation();
  const params = new URLSearchParams(location.search);
  const redirectTo = params.get("callbackUrl") ?? "/";
  const actionData = useActionData() as { error: string } | undefined;

  return (
    <Container component="main" maxWidth="xs" sx={{ mt: 8 }}>
      <Stack spacing={2} alignItems="center">
        <Typography variant="h5">Sign in</Typography>

        <Form method="post" replace>
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
        </Form>

        {actionData && actionData.error && (
          <Alert sx={{ mt: 2 }} severity="error">
            {actionData.error}
          </Alert>
        )}
      </Stack>
    </Container>
  );
};

export default SignInPage;
