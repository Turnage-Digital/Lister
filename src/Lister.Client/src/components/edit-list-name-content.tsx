import { Stack, TextField } from "@mui/material";
import React from "react";

interface Props {
  name: string | null;
  onNameChanged: (name: string) => void;
}

const EditListNameContent = ({ name, onNameChanged }: Props) => {
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
      />
    </Stack>
  );
};

export default EditListNameContent;
