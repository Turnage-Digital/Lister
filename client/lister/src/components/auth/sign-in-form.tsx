import {
  Alert,
  Box,
  Button,
  Container,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import React, { FormEvent, useState } from "react";

interface Props {
  signIn: (username: string, password: string) => Promise<void>;
  error: string | null;
}

const SignInForm = ({ signIn, error }: Props) => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();

    if (username && password) {
      await signIn(username, password);
    }
  };

  return (
    <Container component="main" maxWidth="xs" sx={{ mt: 8 }}>
      <Stack spacing={2} alignItems="center">
        <Typography variant="h5">Sign in</Typography>

        <Box component="form" onSubmit={handleSubmit}>
          <TextField
            margin="normal"
            required
            fullWidth
            name="username"
            id="username"
            label="Username"
            autoComplete="username"
            onChange={(event) => setUsername(event.target.value)}
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
            onChange={(event) => setPassword(event.target.value)}
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
          <Alert sx={{ mt: 2 }} severity="error">
            {error}
          </Alert>
        )}
      </Stack>
    </Container>
  );
};

export default SignInForm;
