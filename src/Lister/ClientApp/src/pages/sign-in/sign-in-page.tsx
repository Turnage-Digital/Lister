import React, { useState } from "react";
import {
  Alert,
  Box,
  Button,
  Container,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { ActionFunctionArgs, useLocation, useNavigate } from "react-router-dom";

import { useAuth } from "../../auth";
import { Loading } from "../../components";

const SignInPage = () => {
  const { loading, error, signIn } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");

  const handleSubmit = async () => {
    if (username && password) {
      const succeeded = await signIn(username, password);

      if (succeeded) {
        const params = new URLSearchParams(location.search);

        navigate(params.get("callbackUrl") || "/");
      }
    }
  };

  if (loading) return <Loading />;

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

export default SignInPage;
