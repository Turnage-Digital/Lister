import * as React from "react";
import { FormEvent, useEffect, useState } from "react";

import { Save } from "@mui/icons-material";
import {
  Box,
  Button,
  Divider,
  Stack,
  Checkbox,
  FormControlLabel,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Typography,
} from "@mui/material";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { createSearchParams, useNavigate } from "react-router-dom";

import {
  EditListColumnsContent,
  EditListNameContent,
  EditListStatusesContent,
  FormBlock,
  Titlebar,
} from "../components";
import { ListItemDefinition } from "../models";

const CreateListPage = () => {
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const createListMutation = useMutation({
    mutationFn: async (list: ListItemDefinition) => {
      const request = new Request("/api/lists", {
        headers: {
          "Content-Type": "application/json",
        },
        method: "POST",
        body: JSON.stringify(list),
      });
      const response = await fetch(request);
      const retval: ListItemDefinition = await response.json();
      return retval;
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries();
    },
  });

  const defaultListItemDefinition: ListItemDefinition = {
    id: null,
    name: "",
    columns: [],
    statuses: [],
    transitions: [],
  };

  const [updated, setUpdated] = useState<ListItemDefinition>(() => {
    const item = window.sessionStorage.getItem("updated_list");
    return item ? JSON.parse(item) : defaultListItemDefinition;
  });

  useEffect(() => {
    window.sessionStorage.setItem("updated_list", JSON.stringify(updated));
  }, [updated]);

  const update = (key: keyof ListItemDefinition, value: unknown) => {
    setUpdated((prev) => ({ ...prev, [key]: value }) as ListItemDefinition);
  };

  const [matrix, setMatrix] = useState<Record<string, Record<string, boolean>>>(
    {},
  );
  useEffect(() => {
    setMatrix((prev) => {
      const names = updated.statuses.map((s) => s.name);
      const rows: Record<string, Record<string, boolean>> = {};
      names.forEach((from) => {
        rows[from] = {} as Record<string, boolean>;
        names.forEach((to) => {
          const prevRow = (prev[from] ?? {}) as Record<string, boolean>;
          rows[from][to] = Boolean(prevRow[to] ?? false);
        });
      });
      return rows;
    });
  }, [updated.statuses]);

  const toggle =
    (from: string, to: string) => (e: React.ChangeEvent<HTMLInputElement>) => {
      setMatrix((prev) => ({
        ...prev,
        [from]: { ...prev[from], [to]: e.target.checked },
      }));
    };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    const transitions = Object.entries(matrix).map(([from, row]) => ({
      from,
      allowedNext: Object.entries(row)
        .filter(([, val]) => Boolean(val))
        .map(([to]) => to),
    }));

    const payload: ListItemDefinition = { ...updated, transitions };

    const mutated = await createListMutation.mutateAsync(payload);
    if (mutated.id === null) {
      throw new Error("List was not created.");
    }

    window.sessionStorage.removeItem("updated_list");
    const search = createSearchParams({ page: "0", pageSize: "10" }).toString();
    navigate(`/${mutated.id}?${search}`);
  };

  const breadcrumbs = [
    {
      title: "Lists",
      onClick: () => navigate("/"),
    },
  ];

  return (
    <Stack
      component="form"
      divider={<Divider />}
      onSubmit={handleSubmit}
      sx={{ px: 2, py: 4 }}
      spacing={4}
    >
      <Titlebar title="Create a List" breadcrumbs={breadcrumbs} />

      <FormBlock
        title="Name"
        content={
          <EditListNameContent
            name={updated.name}
            onNameChanged={(name) => update("name", name)}
          />
        }
      />

      <FormBlock
        title="Columns"
        content={
          <EditListColumnsContent
            columns={updated.columns}
            onColumnsChanged={(columns) => update("columns", columns)}
          />
        }
      />

      <FormBlock
        title="Statuses"
        content={
          <EditListStatusesContent
            statuses={updated.statuses}
            onStatusesChanged={(statuses) => update("statuses", statuses)}
          />
        }
      />

      <FormBlock
        title="Status Transitions"
        content={
          <TableContainer component={Paper}>
            <Table size="small">
              <TableHead>
                <TableRow>
                  <TableCell>From \\ To</TableCell>
                  {updated.statuses.map((s) => (
                    <TableCell key={s.name}>{s.name}</TableCell>
                  ))}
                </TableRow>
              </TableHead>
              <TableBody>
                {updated.statuses.map((s) => (
                  <TableRow key={s.name}>
                    <TableCell>
                      <Typography fontWeight={600}>{s.name}</Typography>
                    </TableCell>
                    {updated.statuses.map((t) => (
                      <TableCell key={`${s.name}-${t.name}`} align="center">
                        {(() => {
                          const row =
                            matrix[s.name] ?? ({} as Record<string, boolean>);
                          const checked = Boolean(row[t.name] ?? false);
                          return (
                            <FormControlLabel
                              control={
                                <Checkbox
                                  checked={checked}
                                  onChange={toggle(s.name, t.name)}
                                />
                              }
                              label=""
                            />
                          );
                        })()}
                      </TableCell>
                    ))}
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
        }
      />

      <Box
        sx={{
          display: "flex",
          justifyContent: { xs: "center", md: "flex-end" },
        }}
      >
        <Button
          type="submit"
          variant="contained"
          startIcon={<Save />}
          loading={createListMutation.isPending}
          sx={{ width: { xs: "100%", md: "auto" } }}
        >
          Submit
        </Button>
      </Box>
    </Stack>
  );
};

export default CreateListPage;
