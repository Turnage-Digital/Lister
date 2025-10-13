import * as React from "react";

import { Stack, TextField } from "@mui/material";

interface Props {
  name: string | null;
  onNameChanged: (name: string) => void;
  disabled?: boolean;
}

const EditListNameContent = ({ name, onNameChanged, disabled }: Props) => {
  return (
    <Stack spacing={2}>
      <TextField
        name="name"
        id="name"
        label="Name"
        margin="normal"
        required
        fullWidth
        sx={{
          background: "white",
        }}
        value={name ?? ""}
        onChange={(event) => {
          onNameChanged(event.target.value);
        }}
        disabled={disabled}
      />
    </Stack>
  );
};

export default EditListNameContent;
