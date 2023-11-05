import React, { useState } from "react";
import {
  Box,
  Button,
  FormControl,
  IconButton,
  InputLabel,
  MenuItem,
  Select,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
} from "@mui/material";
import { Delete } from "@mui/icons-material";

import { Column, ColumnType } from "../../api";

interface Props {
  columns: Column[] | null;
  onColumnsChanged: (columns: Column[]) => void;
}

const ColumnsBlock = ({ columns, onColumnsChanged }: Props) => {
  const [columnName, setColumnName] = useState<string | null>(null);
  const [columnType, setColumnType] = useState<ColumnType | null>(null);

  const handleAddClicked = () => {
    const notNull = columns ?? [];
    const added = [...notNull, { name: columnName!, type: columnType! }];

    setColumnName(null);
    setColumnType(null);
    onColumnsChanged(added);
  };

  const handleRemoveClicked = (name: string) => {
    const notNull = columns ?? [];
    const updated = notNull.filter((pd) => pd.name !== name);

    onColumnsChanged(updated);
  };

  const columnTypeList = Object.values(ColumnType);

  return (
    <Stack spacing={2}>
      <Stack direction="row" spacing={2}>
        <TextField
          name="name"
          id="name"
          label="Name"
          margin="normal"
          fullWidth
          value={columnName ?? ""}
          onChange={(event) => setColumnName(event.target.value)}
          sx={{
            background: "white",
          }}
        />

        <FormControl variant="outlined" margin="normal" fullWidth>
          <InputLabel htmlFor="type">Type</InputLabel>
          <Select
            name="type"
            id="type"
            label="Type"
            value={columnType ?? ""}
            onChange={(event) =>
              setColumnType(event.target.value as ColumnType)
            }
            sx={{
              background: "white",
            }}
          >
            {columnTypeList.map((columnType) => (
              <MenuItem key={columnType} value={columnType}>
                {columnType}
              </MenuItem>
            ))}
          </Select>
        </FormControl>

        <Box sx={{ display: "flex", alignItems: "center" }}>
          <Button
            variant="contained"
            color="primary"
            onClick={handleAddClicked}
            disabled={!columnName || !columnType}
          >
            Add
          </Button>
        </Box>
      </Stack>

      {columns && columns.length > 0 && (
        <TableContainer>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Name</TableCell>
                <TableCell>Type</TableCell>
                <TableCell />
              </TableRow>
            </TableHead>
            <TableBody>
              {columns.map((column) => (
                <TableRow key={column.name}>
                  <TableCell>{column.name}</TableCell>
                  <TableCell>{column.type}</TableCell>
                  <TableCell>
                    <IconButton
                      onClick={() => handleRemoveClicked(column.name)}
                    >
                      <Delete />
                    </IconButton>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}
    </Stack>
  );
};

export default ColumnsBlock;
