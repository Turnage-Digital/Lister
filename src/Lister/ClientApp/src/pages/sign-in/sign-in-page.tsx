import React from "react";
import {
  Alert,
  Button,
  Container,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import {
  ActionFunctionArgs,
  Form,
  redirect,
  useActionData,
  useLocation,
} from "react-router-dom";

export const signInPageAction = async ({ request }: ActionFunctionArgs) => {
  const data = await request.formData();

  const username = data.get("username") as string | null;
  if (!username) {
    return {
      error: "You must provide a username to log in",
    };
  }

  const password = data.get("password") as string | null;
  if (!password) {
    return {
      error: "You must provide a password to log in",
    };
  }

  const input = { username, password };
  const postRequest = new Request(
    `${process.env.PUBLIC_URL}/api/users/sign-in`,
    {
      headers: {
        "Content-Type": "application/json",
      },
      method: "POST",
      body: JSON.stringify(input),
    }
  );
  const response = await fetch(postRequest);
  const { succeeded } = await response.json();
  if (!succeeded) {
    return {
      error: "Invalid username or password",
    };
  }

  const redirectTo = data.get("redirectTo") as string | null;
  return redirect(redirectTo || "/");
};

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
            sx={{
              background: "white",
            }}
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
            sx={{
              background: "white",
            }}
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
