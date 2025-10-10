import * as React from "react";

import {
  Cancel,
  CheckCircle,
  Flag,
  RadioButtonUnchecked,
  Schedule,
} from "@mui/icons-material";
import { alpha, Chip, darken, useTheme } from "@mui/material";

import { Status } from "../models";

interface Props {
  status?: Status;
  onDelete?: () => void;
}

const StatusChip = ({ status, onDelete }: Props) => {
  const theme = useTheme();

  // Get appropriate icon based on status name (you can customize this logic)
  const getStatusIcon = (statusName: string) => {
    const name = statusName.toLowerCase();
    if (
      name.includes("complete") ||
      name.includes("done") ||
      name.includes("finished")
    ) {
      return <CheckCircle sx={{ fontSize: 16 }} />;
    }
    if (
      name.includes("progress") ||
      name.includes("pending") ||
      name.includes("active")
    ) {
      return <Schedule sx={{ fontSize: 16 }} />;
    }
    if (
      name.includes("cancel") ||
      name.includes("reject") ||
      name.includes("failed")
    ) {
      return <Cancel sx={{ fontSize: 16 }} />;
    }
    if (
      name.includes("new") ||
      name.includes("open") ||
      name.includes("todo")
    ) {
      return <RadioButtonUnchecked sx={{ fontSize: 16 }} />;
    }
    return <Flag sx={{ fontSize: 16 }} />;
  };

  const [isAnimating, setIsAnimating] = React.useState(false);

  React.useEffect(() => {
    if (!status) return;
    // Trigger entrance animation
    setIsAnimating(true);
    const timer = setTimeout(() => setIsAnimating(false), 300);
    return () => clearTimeout(timer);
  }, [status]);

  if (!status) {
    return null;
  }

  return (
    <Chip
      icon={getStatusIcon(status.name)}
      label={status.name}
      size="small"
      sx={{
        color: darken(status.color, 0.6),
        fontWeight: 600,
        fontSize: "0.75rem",
        backgroundColor: alpha(status.color, 0.1),
        border: `1px solid ${alpha(status.color, 0.3)}`,
        transform: isAnimating ? "scale(1.05)" : "scale(1)",
        animation: isAnimating ? "statusPulse 0.3s ease-out" : "none",
        "@keyframes statusPulse": {
          "0%": {
            transform: "scale(1)",
            backgroundColor: alpha(status.color, 0.1),
          },
          "50%": {
            transform: "scale(1.05)",
            backgroundColor: alpha(status.color, 0.2),
          },
          "100%": {
            transform: "scale(1)",
            backgroundColor: alpha(status.color, 0.1),
          },
        },
        "& .MuiChip-icon": {
          color: darken(status.color, 0.4),
          transition: `all ${theme.transitions.duration.short}ms ${theme.transitions.easing.easeOut}`,
        },
        "&:hover": {
          backgroundColor: alpha(status.color, 0.15),
          transform: "translateY(-1px)",
          boxShadow: `0 4px 8px ${alpha(status.color, 0.2)}`,
          "& .MuiChip-icon": {
            transform: "scale(1.1)",
          },
        },
        "&:active": {
          transform: "translateY(0px) scale(0.98)",
        },
        transition: `all ${theme.transitions.duration.short}ms ${theme.transitions.easing.easeOut}`,
      }}
      onDelete={onDelete}
    />
  );
};

export default StatusChip;
