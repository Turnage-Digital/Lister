import * as React from "react";

import { Delete, Visibility, List as ListIcon } from "@mui/icons-material";
import {
  Card,
  CardActions,
  CardContent,
  IconButton,
  Tooltip,
  Typography,
  Box,
  Chip,
  alpha,
  useTheme,
} from "@mui/material";

import { ListName } from "../models";
import PreloadIconButton from "./preload-icon-button";

interface Props {
  listName: ListName;
  onDeleteClick: (id: string) => void;
}

const ListCard = ({ listName, onDeleteClick }: Props) => {
  const theme = useTheme();

  return (
    <Card
      variant="outlined"
      sx={{
        height: "100%",
        display: "flex",
        flexDirection: "column",
        position: "relative",
        transition: "all 0.2s ease-in-out",
        cursor: "pointer",
        "&:hover": {
          transform: "translateY(-4px)",
          boxShadow: theme.shadows[8],
          borderColor: alpha(theme.palette.primary.main, 0.5),
          "& .card-actions": {
            opacity: 1,
          },
        },
      }}
    >
      {/* Header with icon and count */}
      <Box
        sx={{
          p: 3,
          pb: 2,
          backgroundColor: alpha(theme.palette.primary.main, 0.03),
          borderRadius: `${theme.shape.borderRadius}px ${theme.shape.borderRadius}px 0 0`,
        }}
      >
        <Box sx={{ display: "flex", alignItems: "center", gap: 1, mb: 2 }}>
          <ListIcon
            sx={{
              fontSize: 20,
              color: "primary.main",
              p: 0.5,
              backgroundColor: alpha(theme.palette.primary.main, 0.1),
              borderRadius: 1,
            }}
          />
          <Chip
            label={`${listName.count} items`}
            size="small"
            variant="outlined"
            sx={{
              fontSize: "0.75rem",
              fontWeight: 500,
              ml: "auto",
            }}
          />
        </Box>

        <Typography
          variant="h6"
          component="div"
          fontWeight="600"
          sx={{
            fontSize: "1.25rem",
            lineHeight: 1.3,
            color: "text.primary",
          }}
        >
          {listName.name}
        </Typography>
      </Box>

      {/* Content area - could be expanded later */}
      <CardContent sx={{ flex: 1, pt: 1 }}>
        <Typography variant="body2" color="text.secondary">
          Manage and organize your {listName.name.toLowerCase()} items
        </Typography>
      </CardContent>

      {/* Action buttons */}
      <CardActions
        className="card-actions"
        sx={{
          justifyContent: "flex-end",
          p: 2,
          pt: 0,
          opacity: 0.7,
          transition: "opacity 0.2s ease-in-out",
        }}
      >
        <Tooltip title={`View ${listName.name}`}>
          <PreloadIconButton
            to="/$listId"
            params={{ listId: listName.id }}
            search={{ page: 0, pageSize: 10 }}
            preload="intent"
            sx={{
              color: "primary.main",
              "&:hover": {
                backgroundColor: alpha(theme.palette.primary.main, 0.1),
              },
            }}
          >
            <Visibility />
          </PreloadIconButton>
        </Tooltip>
        <Tooltip title={`Delete ${listName.name}`}>
          <IconButton
            onClick={() => onDeleteClick(listName.id)}
            sx={{
              color: "error.main",
              "&:hover": {
                backgroundColor: alpha(theme.palette.error.main, 0.1),
              },
            }}
          >
            <Delete />
          </IconButton>
        </Tooltip>
      </CardActions>
    </Card>
  );
};

export default ListCard;
