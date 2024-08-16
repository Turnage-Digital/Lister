import React, { FormEvent, useState } from "react";
import {
  Box,
  Button,
  Dialog,
  Stack,
  TextField,
  Typography,
} from "@mui/material";

import useAuth from "./use-auth";

const SignInDialog = () => {
  const { loggedIn, login } = useAuth();

  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();

    if (username && password) {
      await login(username, password);
    }
  };

  return (
    <Dialog open={!loggedIn} fullScreen>
      <Stack spacing={2} alignItems="center" sx={{ px: 4, py: 8 }}>
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
      </Stack>
    </Dialog>
  );
};

export default SignInDialog;
