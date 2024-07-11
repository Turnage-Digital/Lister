import { Chip, lighten, darken } from "@mui/material";
import React from "react";

import { Status } from "../api";

interface Props {
  status?: Status;
  onDelete?: () => void;
}

const StatusChip = ({ status, onDelete }: Props) => {
  if (!status) {
    return null;
  }

  return (
    <Chip
      label={status.name}
      sx={{
        color: darken(status.color, 0.4),
        fontWeight: "bold",
        backgroundColor: lighten(status.color, 0.6),
      }}
      onDelete={onDelete}
    />
  );
};

export default StatusChip;
