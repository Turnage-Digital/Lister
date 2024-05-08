import { Chip } from "@mui/material";
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
        color: "white",
        backgroundColor: status.color,
      }}
      onDelete={onDelete}
    />
  );
};

export default StatusChip;
