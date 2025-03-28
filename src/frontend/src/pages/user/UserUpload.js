import React, { useState, useRef, useCallback } from 'react';
import Box from '@mui/material/Box';
import { Input, Typography, DatePicker, Button, Form, Select, Tag, Flex, Image, Modal, Slider, message, Result, Spin } from "antd";
import { PlusOutlined, RotateLeftOutlined, RotateRightOutlined, ExclamationCircleOutlined, CalendarOutlined, DownOutlined, CloseOutlined } from '@ant-design/icons';
import Cropper from 'react-easy-crop';
import dayjs from 'dayjs';
import { projects, users } from '../../utils/dummyData.js';
import { addLog } from "../../api/logApi";
import { API_BASE_URL } from '../../api/apiURL.js';
import { useAuth } from '../../contexts/AuthContext.js';
import { Palette } from '@mui/icons-material';
import { useEffect } from "react";

const { Title } = Typography;
const { confirm } = Modal;
const userID = 10;
const projectId = 6;

const tagStyle = {
    backgroundColor: '#dbdbdb',
    padding: '5px 10px',
    margin: '3px'
};

export default function UserUpload() {
    const [files, setFiles] = useState([]);
    const [croppedImages, setCroppedImages] = useState([]);
    const [currentFile, setCurrentFile] = useState(null);
    const [crop, setCrop] = useState({ x: 0, y: 0 });
    const [zoom, setZoom] = useState(1);
    const [rotation, setRotation] = useState(0);
    const [editing, setEditing] = useState(false);
    const MAX_FILES = 100;
    const [taggingMode, setTaggingMode] = useState(false);
    const [selectedFiles, setSelectedFiles] = useState(new Set());
    const [userFiles, setUserFiles] = useState([]);
    const { user } = useAuth();
    const [spinning, setSpinning] = React.useState(false);
    const [percent, setPercent] = React.useState(0);



    const [project, setProject] = useState(null);
    const [metadataTagsInput, setMetadataTagsInput] = useState();
    const [metadataTags, setMetadataTags] = useState([]);
    const [tagApplications, setTagApplications] = useState([]);
    const [location, setLocation] = useState(null);
    const [selectedDate, setSelectedDate] = useState(dayjs().format('YYYY-MM-DD'));
    const fileInputRef = useRef(null);
    const [uploadSuccess, setUploadSuccess] = useState(false);
    const metadataBoxStyle = {
        textAlign: 'left',
        backgroundColor: '#f5f5f5',
        borderRadius: '10px',
        padding: '12px',
        boxShadow: '0 4px 6px rgba(0, 0, 0, 0.1)',
        marginBottom: '12px',
        width: '100%'
    };
    useEffect(() => {
        const fetchPalette = async () => {
            await getUserPalette();
        };

        fetchPalette();
    }, []);

    async function getUserPalette() {
        try {
            const response = await fetch(`${API_BASE_URL}/api/Files/${user.id}/palette`, {
                method: 'GET',
                headers: {
                    'Accept': 'text/plain'
                }
            });

            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }

            const data = await response.json();
            if (data.length) {
                handleProjectChange(data[0].projectId);
                handleDateChange(data[0].dateTimeOriginal);
                setLocation(data[0].location || "");
                setSelectedDate(data[0].dateTimeOriginal.split("T")[0])
            }
            const paletteFiles = data.map((file, index) => (
                {
                    id: file.id,
                    preview: file.viewPath,
                    metadata: file.bTags.length > 0 ? file.bTags.map(item => item.value) : [],
                    date: file.dateTimeOriginal.split("T")[0],
                    location: file.location || "",
                    projectId: file.projectId,
                    userId: file.userId,
                    file: { name: file.name },
                }));
            paletteFiles.forEach(file => {
                if (file.metadata.length) {
                    setTagApplications(prev => [
                        { tags: [...file.metadata], files: [file] }
                    ]);
                }
            });
            console.log("User Palette Details : ", paletteFiles);
            setFiles((prevFiles) => [...paletteFiles]);
            setUserFiles((prevUserFiles) => [...paletteFiles]);

        } catch (error) {
            console.error('Error:', error);
        }
    }
    async function deleteFile(fileId) {
        try {
            const response = await fetch(`${API_BASE_URL}/api/Files/${fileId}`, {
                method: 'DELETE',
                headers: {
                    'Accept': 'text/plain'
                }
            });

            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }

            const data = await response.json();

            console.log("File Deleted : ");

        } catch (error) {
            console.error('Error:', error);
        }
    }

    const handleMetadataTagClose = (removedTag) => {
        const newTags = metadataTags.filter((tag) => tag !== removedTag);
        setMetadataTags(newTags);
    }

    const handleMetadataTagAdd = () => {
        if (metadataTagsInput && !metadataTags.includes(metadataTagsInput)) {
            setMetadataTags([...metadataTags, metadataTagsInput]);
        }
        setMetadataTagsInput("");
    }


    const showDuplicateAlert = (duplicates) => {
        message.warning(`Duplicate files ignored: ${duplicates.join(", ")}`);
    };

    const handleFileChange = async (event) => {
        const selectedFiles = Array.from(event.target.files);
        // Check if any files are selected
        const totalFiles = files.length + selectedFiles.length;

        if (totalFiles > MAX_FILES) {
            message.error(`You can only upload up to ${MAX_FILES} images. You tried adding ${totalFiles}.`);
            return;
        }

        const formData = new FormData();
        selectedFiles.forEach(file => {
            formData.append('files', file);
        });
        try {
            setSpinning(true);
            // Upload the files to the server
            const response = await fetch(`${API_BASE_URL}/api/files/upload`, {
                method: 'POST',
                body: formData,
            });
            setSpinning(false);
            if (!response.ok) {
                throw new Error('Failed to upload files.');
            }

            const uploadedFileUrls = await response.json();

            const newFiles = selectedFiles.map((file, index) => ({
                file: { name: getFileName(uploadedFileUrls[index]) },
                preview: uploadedFileUrls[index] || URL.createObjectURL(file),
                metadata: [],
                date: selectedDate || null,
                location: location || "",
                projectId: project ? project.id : null,
                userId: user.id
            }));

            setFiles((prevFiles) => [...prevFiles, ...newFiles]);
            setUserFiles((prevUserFiles) => [...prevUserFiles, ...newFiles]);
        } catch (error) {
            console.error(error);
            message.error('An error occurred while uploading files.');
        }
    };
    const getFileName = (url) => {
        return url.split('/').pop().split('?')[0].split('#')[0];
    };
    const handleEditImage = (file) => {
        setCurrentFile(file);
        setEditing(true);
    };

    const onCropComplete = useCallback((croppedArea, croppedAreaPixels) => {
        console.log(croppedArea, croppedAreaPixels);
    }, []);

    const saveEditedImage = () => {
        setCroppedImages([...croppedImages, currentFile]);
        setEditing(false);
        setCurrentFile(null);
    };

    const confirmRemoveFile = (file) => {
        if (!file || !file.file) {
            console.error("Invalid file object:", file);
            return;
        }
        confirm({
            title: 'Are you sure you want to remove this image?',
            icon: <ExclamationCircleOutlined />,
            content: 'This action cannot be undone.',
            okText: 'Yes, remove',
            okType: 'danger',
            cancelText: 'No',
            onOk() {
                removeFile(file);
            }
        });

    };

    const removeFile = async (file) => {
        if (file.id) {
            setSpinning(true);
            await deleteFile(file.id);
            setSpinning(false);
        }
        setFiles(prevFiles => {
            const updatedFiles = prevFiles.filter(f => f.file.name !== file.file.name);
            return [...updatedFiles];
        });
        setUserFiles(prevFiles => prevFiles.filter(f => f.file.name !== file.file.name));

    };

    const handleProjectChange = (value) => {
        const selectedProject = projects.find(proj => proj.id === value);
        setProject(selectedProject);
        setFiles(prevFiles => prevFiles.map(file => ({ ...file, projectId: selectedProject.id })));
    };


    const handleToggleTagging = () => {
        setTaggingMode((prev) => !prev);
        setSelectedFiles(new Set());
    };


    const toggleFileSelection = (fileObj) => {

        const fileName = fileObj.file.name;
        const updatedSelection = new Set(selectedFiles);

        if (updatedSelection.has(fileName)) {
            updatedSelection.delete(fileName);
        } else {
            updatedSelection.add(fileName);
        }

        setSelectedFiles(updatedSelection);
    };

    const handleTagAllFiles = () => {
        setTaggingMode(true);
        const allFiles = new Set(files.map(({ file }) => file.name));
        setSelectedFiles(allFiles);
    };

    const handleSubmitTagInfo = () => {
        if (selectedFiles.size === 0 || metadataTags.length === 0) return;

        setFiles(prevFiles =>
            prevFiles.map(fileObj => {
                if (selectedFiles.has(fileObj.file.name)) {
                    return {
                        ...fileObj,
                        metadata: [...new Set([...fileObj.metadata, ...metadataTags])]
                    };
                }
                return fileObj;
            })
        );

        setTagApplications(prev => [
            ...prev,
            { tags: [...metadataTags], files: [...selectedFiles] }
        ]);

        setSelectedFiles(new Set());
        setMetadataTags([]);
        setTaggingMode(false);
    };

    const removeTagApplication = (index) => {
        const { tags, files } = tagApplications[index];

        setFiles(prevFiles =>
            prevFiles.map(fileObj => {
                if (files.includes(fileObj.file.name)) {
                    return {
                        ...fileObj,
                        metadata: fileObj.metadata.filter(tag => !tags.includes(tag))
                    };
                }
                return fileObj;
            })
        );

        setTagApplications(prev => prev.filter((_, i) => i !== index));
    };

    const handleDateChange = (date, dateString) => {
        setSelectedDate(dateString);
        setFiles(prevFiles => prevFiles.map(file => ({ ...file, date: dateString })));
    };

    const handleLocationChange = (event) => {
        const newLocation = event.target.value;
        setLocation(newLocation);
        setFiles(prevFiles => prevFiles.map(file => ({ ...file, location: newLocation })));
    };
    const handleUploadFilesToPalette = async () => {
        console.log("Uploading files to palette:", files);
        setSpinning(true);
        const filesToSave = files.map(({ file, ...rest }) => ({
            ...rest,
            filePath: rest.preview,
            palette: true
        }));
        await saveFiles(filesToSave);
        await getUserPalette();
        setSpinning(false);
    }
    const saveFiles = async (filesToSave) => {
        try {
            // Upload the files to the server
            const response = await fetch(`${API_BASE_URL}/api/files`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Accept': 'application/json'
                },
                body: JSON.stringify(filesToSave),
            });

            if (!response.ok) {
                throw new Error('Failed to upload files.');
            }

            // Assuming the API returns an array of URLs corresponding to the uploaded files
            const uploadedFileUrls = await response.json();
        } catch (error) {
            console.error('Error:', error);
        }
    }
    const handleUploadFilesToProject = async () => {
        // TODO: add "files" to current "project"'s "files" variable, and other associated info
        // TODO: update user's activity log that they added files to this certain project

        console.log("Uploading files:", files);
        setSpinning(true);
        const filesToSave = files.map(({ file, ...rest }) => ({
            ...rest,
            filePath: rest.preview,
            palette: false
        }));
        await saveFiles(filesToSave);
        setSpinning(false);
        userFiles.forEach(file => {
            addLog(userID, file.id, projectId, 'upload');
        });


        setFiles([]);
        setUserFiles([]);
        setTagApplications([]);
        setProject(null);
        setMetadataTags([]);
        setSelectedDate(dayjs().format('YYYY-MM-DD'));
        setLocation(null);
        setUploadSuccess(true);
        addLog(userID, 3, 'upload');

    };

    const resetUploadState = () => {
        setUploadSuccess(false);
    };

    return (
        <Box sx={{ display: 'flex', flexDirection: 'row', minHeight: '100vh', padding: '20px', gap: '20px', paddingBottom: '40px' }}>
            {/* Left section (image uploading)*/}
            <Box sx={{ flex: 3, display: 'flex', flexDirection: 'column', gap: '15px', padding: '20px' }}>
                <Box sx={{
                    textAlign: 'center',
                    padding: 4,
                    backgroundColor: '#f5f5f5',
                    borderRadius: '10px',
                    boxShadow: '0 4px 6px rgba(0, 0, 0, 0.1)',
                }}>
                    <h2>Upload & Edit Files</h2>
                    <p style={{ color: files.length >= MAX_FILES ? 'red' : 'black' }}>
                        {files.length}/{MAX_FILES} images uploaded
                    </p>
                    <input
                        type="file"
                        multiple
                        accept="image/*"
                        ref={fileInputRef}
                        onChange={handleFileChange}
                        style={{ display: 'none' }}
                    />
                    <Button icon={<PlusOutlined />} type="primary" color="cyan" variant="solid" onClick={() => fileInputRef.current.click()} disabled={files.length >= MAX_FILES}>
                        Add Files
                    </Button>
                </Box>

                {/* Image preview & edit options */}
                <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 2, padding: 4 }}>
                    {files.map(({ file, preview }, index) => (
                        <div key={index} style={{ position: 'relative', width: '150px' }}>
                            <Image src={preview} width={150} preview={false} />
                            {taggingMode && (
                                <div
                                    onClick={() => toggleFileSelection(files[index])}
                                    style={{
                                        position: 'absolute',
                                        top: '5px',
                                        right: '5px',
                                        width: '24px',
                                        height: '24px',
                                        borderRadius: '50%',
                                        backgroundColor: 'white',
                                        display: 'flex',
                                        alignItems: 'center',
                                        justifyContent: 'center',
                                        fontSize: '16px',
                                        fontWeight: 'bold',
                                        color: 'black',
                                        cursor: 'pointer',
                                    }}
                                >
                                    {selectedFiles.has(files[index].file.name) && '✔️'}
                                </div>
                            )}

                            <Button size="small" onClick={() => handleEditImage({ file, preview })}>Edit</Button>
                            <Button danger size="small" onClick={() => {
                                confirmRemoveFile(files[index]);
                            }}>
                                Remove
                            </Button>

                        </div>
                    ))}
                </Box>

                {/* Image editor popup */}
                <Modal
                    open={editing}
                    onCancel={() => setEditing(false)}
                    onOk={saveEditedImage}
                    okText="Save Changes"
                    title="Edit Image"
                    width={600}
                >
                    {currentFile && (
                        <div style={{ width: '100%', height: 400, position: 'relative' }}>
                            <Cropper
                                image={currentFile.preview}
                                crop={crop}
                                zoom={zoom}
                                rotation={rotation}
                                onCropChange={setCrop}
                                onZoomChange={setZoom}
                                onRotationChange={setRotation}
                                onCropComplete={onCropComplete}
                            />
                            <Box sx={{ display: 'flex', justifyContent: 'center', gap: 2, marginTop: 2 }}>
                                <Button icon={<RotateLeftOutlined />} onClick={() => setRotation(rotation - 90)} />
                                <Button icon={<RotateRightOutlined />} onClick={() => setRotation(rotation + 90)} />
                                <Slider min={1} max={3} step={0.1} value={zoom} onChange={setZoom} />
                            </Box>
                        </div>
                    )}
                </Modal>

                {/* Upload button w success popup*/}
                <Box sx={{ textAlign: 'center', marginTop: '20px' }}>
                    {uploadSuccess ? (
                        <Result
                            status="success"
                            title="Files Successfully Uploaded!"
                            subTitle={"Your files have been added!"}
                            extra={[
                                <Button key="uploadAgain" onClick={resetUploadState}>
                                    Return
                                </Button>,
                            ]}
                        />
                    ) : (
                        <div style={{ display: 'flex', justifyContent: 'center' }}>
                            <Button style={{ margin: '10px' }} type="primary" color="cyan" variant="solid" onClick={handleUploadFilesToProject} disabled={files.length === 0 || project === null}>
                                Upload Files to Project
                            </Button>
                            <Button style={{ margin: '10px' }} type="secondary" color="green" variant="solid" disabled={files.length === 0 || project === null} onClick={handleUploadFilesToPalette}>
                                Save To Palette
                            </Button>
                        </div>
                    )}
                </Box>

            </Box>

            {/* Right section (metadata) */}
            <Box sx={{ flex: 1, display: 'flex', flexDirection: 'column', gap: '12px', padding: '30px' }}>
                <Box sx={metadataBoxStyle}>
                    <Title level={5}>Project Name:</Title>
                    <Select
                        showSearch
                        placeholder="Enter project number"
                        filterOption={(input, option) =>
                            (option?.label ?? '').toLowerCase().includes(input.toLowerCase())
                        }
                        options={projects.map(proj => ({
                            value: proj.id,
                            label: `${proj.id}: ${proj.name}`
                        }))}
                        onChange={handleProjectChange}
                        style={{ width: '100%' }}
                        value={project !== null ? project.id : undefined}
                    />
                </Box>

                <Box sx={metadataBoxStyle}>
                    <Box sx={{ display: 'flex', justifyContent: 'flex-end', gap: '10px' }}>
                        <Button type="primary" color="cyan" variant="solid" onClick={handleToggleTagging} disabled={files.length === 0}>
                            {taggingMode ? "Cancel Tagging" : "Tag Files"}
                        </Button>
                        <Button type="primary" color="cyan" variant="solid" onClick={handleTagAllFiles} disabled={files.length === 0}>
                            Tag All
                        </Button>
                    </Box>
                    <Title level={5}>Add Metadata:</Title>
                    <Input
                        placeholder="Select files first"
                        value={metadataTagsInput}
                        onChange={(e) => setMetadataTagsInput(e.target.value)}
                        onPressEnter={handleMetadataTagAdd}
                        disabled={selectedFiles.size === 0}
                    />
                    <Flex wrap="wrap" style={{ marginTop: '10px' }}>
                        {metadataTags.map((tag) => (
                            <Tag
                                style={tagStyle}
                                key={tag}
                                closable={true}
                                onClose={() => handleMetadataTagClose(tag)}
                            >
                                {tag}
                            </Tag>
                        ))
                        }
                    </Flex>
                    <Box sx={{ marginTop: '15px' }}>
                        <Button type="primary" color="cyan" variant="solid" onClick={handleSubmitTagInfo} disabled={selectedFiles.size === 0 || metadataTags.length === 0 || files.length === 0}>
                            Submit Tag Info
                        </Button>
                    </Box>
                    <Box sx={{ marginTop: "20px" }}>
                        {tagApplications.map(({ tags, files }, index) => (
                            <Box key={index} sx={{ backgroundColor: "#eef", padding: "10px", marginTop: "10px", borderRadius: "8px" }}>
                                <Flex justify="space-between">
                                    <div>
                                        <strong>Tags:</strong> {tags.join(", ")} <br />
                                        <strong>Files:</strong> {files.length} {files.length === 1 ? "file" : "files"}

                                    </div>
                                    <Button type="text" icon={<CloseOutlined />} danger onClick={() => removeTagApplication(index)}>
                                        Undo
                                    </Button>
                                </Flex>
                            </Box>
                        ))}
                    </Box>
                </Box>

                <Box sx={metadataBoxStyle}>
                    <Title level={5}>Adjust Resolution:</Title>
                    <Select
                        placeholder="Select resolution"
                        style={{ width: '100%' }}
                        options={[
                            { value: 'low', label: 'Low' },
                            { value: 'medium', label: 'Medium' },
                            { value: 'high', label: 'High' }
                        ]}
                        suffixIcon={<DownOutlined />}
                    />
                </Box>

                <Box sx={metadataBoxStyle}>
                    <Title level={5}>Add Date:</Title>
                    <DatePicker
                        placeholder="Select date"
                        maxDate={dayjs()}
                        onChange={handleDateChange}
                        suffixIcon={<CalendarOutlined />}
                        style={{ width: '100%' }}
                        value={selectedDate ? dayjs(selectedDate) : null}
                    />
                </Box>

                <Box sx={metadataBoxStyle}>
                    <Title level={5}>Location:</Title>
                    <Input
                        value={location}
                        onChange={handleLocationChange}
                        placeholder="Enter location"
                    />
                </Box>
            </Box>
            <Spin spinning={spinning} fullscreen tip="Please Wait..." size="large" />
        </Box>

    );
}