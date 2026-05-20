import React, { useState } from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  IconButton,
  Typography,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Chip,
} from '@mui/material';
import { Close as CloseIcon, Keyboard as KeyboardIcon } from '@mui/icons-material';
import {
  useKeyboardShortcut,
  commonShortcuts,
  type KeyboardShortcut,
  formatShortcut,
} from '../hooks/useKeyboardShortcut';

interface ShortcutGroup {
  title: string;
  shortcuts: KeyboardShortcut[];
}

interface KeyboardShortcutsDialogProps {
  shortcutGroups?: ShortcutGroup[];
}

export const KeyboardShortcutsDialog: React.FC<KeyboardShortcutsDialogProps> = ({
  shortcutGroups = [],
}) => {
  const [open, setOpen] = useState(false);

  const handleOpen = () => setOpen(true);
  const handleClose = () => setOpen(false);

  // Register help shortcut (Shift+?)
  useKeyboardShortcut(commonShortcuts.help(handleOpen));

  // Default shortcuts
  const defaultShortcuts: ShortcutGroup[] = [
    {
      title: 'Global Shortcuts',
      shortcuts: [
        { key: '?', shift: true, callback: () => {}, description: 'Show keyboard shortcuts' },
        { key: 'n', ctrl: true, callback: () => {}, description: 'New item (context-dependent)' },
        { key: 's', ctrl: true, callback: () => {}, description: 'Save (context-dependent)' },
        { key: 'f', ctrl: true, callback: () => {}, description: 'Search (context-dependent)' },
        { key: 'Escape', callback: () => {}, description: 'Close dialog/Cancel action' },
      ],
    },
  ];

  const allGroups = [...defaultShortcuts, ...shortcutGroups];

  return (
    <>
      {/* Help Icon Button */}
      <IconButton
        onClick={handleOpen}
        sx={{
          position: 'fixed',
          bottom: 16,
          right: 16,
          bgcolor: 'primary.main',
          color: 'white',
          '&:hover': {
            bgcolor: 'primary.dark',
          },
          zIndex: 1000,
        }}
        aria-label="Show keyboard shortcuts"
      >
        <KeyboardIcon />
      </IconButton>

      {/* Dialog */}
      <Dialog open={open} onClose={handleClose} maxWidth="md" fullWidth>
        <DialogTitle>
          <Typography variant="h6" component="span">
            Keyboard Shortcuts
          </Typography>
          <IconButton
            onClick={handleClose}
            sx={{
              position: 'absolute',
              right: 8,
              top: 8,
            }}
          >
            <CloseIcon />
          </IconButton>
        </DialogTitle>

        <DialogContent dividers>
          {allGroups.map((group, index) => (
            <div key={index}>
              <Typography variant="h6" gutterBottom sx={{ mt: index > 0 ? 3 : 0 }}>
                {group.title}
              </Typography>
              <TableContainer component={Paper} variant="outlined" sx={{ mb: 2 }}>
                <Table size="small">
                  <TableHead>
                    <TableRow>
                      <TableCell>
                        <strong>Shortcut</strong>
                      </TableCell>
                      <TableCell>
                        <strong>Description</strong>
                      </TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {group.shortcuts.map((shortcut, idx) => (
                      <TableRow key={idx}>
                        <TableCell>
                          <Chip label={formatShortcut(shortcut)} size="small" />
                        </TableCell>
                        <TableCell>{shortcut.description}</TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </TableContainer>
            </div>
          ))}

          <Typography variant="body2" color="text.secondary" sx={{ mt: 2 }}>
            Tip: Press <Chip label="Shift+?" size="small" /> anytime to view this list.
          </Typography>
        </DialogContent>
      </Dialog>
    </>
  );
};
