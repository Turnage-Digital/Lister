import * as React from "react";

import {
  Delete,
  Edit,
  List as ListIcon,
  Visibility,
} from "@mui/icons-material";
import {
  alpha,
  Box,
  Card,
  CardActions,
  Chip,
  IconButton,
  Stack,
  Tooltip,
  Typography,
  useTheme,
} from "@mui/material";

import { Link as RouterLink } from "react-router-dom";

import { ListName } from "../models";

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
        display: "flex",
        flexDirection: "column",
        minHeight: 220,
        p: 3,
        gap: 3,
        borderColor: alpha(theme.palette.primary.main, 0.08),
        transition: "border-color 0.2s ease, box-shadow 0.2s ease",
        "&:hover": {
          borderColor: alpha(theme.palette.primary.main, 0.3),
          boxShadow: theme.shadows[3],
        },
      }}
    >
      <Stack
        direction="row"
        alignItems="center"
        justifyContent="space-between"
        spacing={2}
      >
        <Box
          sx={{
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            width: 44,
            height: 44,
            borderRadius: 2,
            backgroundColor: alpha(theme.palette.primary.main, 0.1),
            color: "primary.main",
          }}
        >
          <ListIcon sx={{ fontSize: 22 }} />
        </Box>
        <Chip
          label={`${listName.count} items`}
          size="small"
          color="primary"
          variant="outlined"
        />
      </Stack>

      <Typography variant="h6" component="div">
        {listName.name}
      </Typography>

      <Box sx={{ flexGrow: 1 }} />

      <CardActions sx={{ justifyContent: "flex-end", gap: 1 }}>
        <Tooltip title={`View ${listName.name}`}>
          <IconButton
            component={RouterLink}
            to={`/${listName.id}?page=0&pageSize=10`}
            color="primary"
          >
            <Visibility />
          </IconButton>
        </Tooltip>
        <Tooltip title={`Edit ${listName.name}`}>
          <IconButton
            component={RouterLink}
            to={`/${listName.id}/edit`}
            color="primary"
          >
            <Edit />
          </IconButton>
        </Tooltip>
        <Tooltip title={`Delete ${listName.name}`}>
          <IconButton onClick={() => onDeleteClick(listName.id)} color="error">
            <Delete />
          </IconButton>
        </Tooltip>
      </CardActions>
    </Card>
  );
};

export default ListCard;
