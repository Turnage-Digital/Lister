import React from "react";
import { Chip } from "@mui/material";

import { Status } from "../models";

interface Props {
  status: Status;
  onDelete?: () => void;
}

const StatusChip = ({ status, onDelete }: Props) => {
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
