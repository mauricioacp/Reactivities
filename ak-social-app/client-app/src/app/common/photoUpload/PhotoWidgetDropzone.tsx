import React, { useCallback } from 'react';
import { useDropzone } from 'react-dropzone';
import { Header, Icon } from 'semantic-ui-react';

interface IProps {
  setFiles: (files: Object[]) => void;
}

const dropZoneStyles = {
  border: 'dashed 3px',
  borderColor: '#eee',
  borderRadius: '5px',
  paddingTop: '30px',
  textAlign: 'center' as 'center',
  height: '200px',
};

const dropzoneActive = {
  borderColor: 'green',
};

const PhotoWidgetDropzone: React.FC<IProps> = ({ setFiles }) => {
  const onDrop = useCallback(
    (acceptedFiles) => {
      setFiles(
        acceptedFiles.map((file: Object) =>
          Object.assign(file, {
            preview: URL.createObjectURL(file),
          }),
        ),
      );
    },
    [setFiles],
  );
  const { getRootProps, getInputProps, isDragActive } = useDropzone({ onDrop });

  return (
    <div
      {...getRootProps()}
      style={isDragActive ? { ...dropZoneStyles, ...dropzoneActive } : dropZoneStyles}
    >
      <input {...getInputProps()} />
      <Icon name='upload' size='huge' />
      <Header content={isDragActive ? 'Release' : 'Drop image here'} />
    </div>
  );
};

export default PhotoWidgetDropzone;
