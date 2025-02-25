import React, { useState, useRef } from 'react';
import Box from '@mui/material/Box';
import { Input, Typography, DatePicker, Button, Form, Select, Tag, Flex } from "antd";
import { CalendarOutlined } from '@ant-design/icons';

const { Title } = Typography;

const tagStyle = {
    backgroundColor: '#dbdbdb'
};

//TODO : some of the boxes use the same formatting as UserDashboard.js, best to abstract styles

export default function UserDashboard() {
    const [projectName, setProjectName] = useState(null);
    const [metadataTagsInput, setMetadataTagsInput] = useState();
    const [metadataTags, setMetadataTags] = useState([]);
    const [location, setLocation] = useState(null);
    const [selectedDate, setSelectedDate] = useState(null);
    const [files, setFiles] = useState([]);
    const fileInputRef = useRef(null);
    const metadataBoxStyle = {
        textAlign: 'left',
        backgroundColor: '#f5f5f5',
        borderRadius: '10px',
        boxShadow: '0 4px 6px rgba(0, 0, 0, 0.1)',
        paddingLeft: 2,
        paddingRight: 2,
    };

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

    const handleFileChange = (event) => {
        const selectedFiles = Array.from(event.target.files);
        setFiles(selectedFiles);
    };

    const openFileWindow = () => {
        fileInputRef.current.click();
    };

    return (
        <Box
            sx={{
                display: 'flex',
                flexDirection: 'column',
                height: '100vh',
            }}
        >
            {/* Title */}
            <Box
                sx={{
                    textAlign: 'center',
                    padding: 4,
                }}
            >
                <Title level={1}>Upload Files</Title>
            </Box>

            {/* Main content */}
            <Box
                sx={{
                    display: 'flex',
                    flexGrow: 1,
                }}
            >
                {/* Left container for upload */}
                <Box
                    sx={{
                        display: 'flex',
                        flexDirection: 'column',
                        justifyContent: 'flex-start',
                        width: '60%',
                        minWidth: '150px',
                        overflow: 'hidden',
                        padding: 4,
                        paddingRight: 2,
                    }}
                >
                    <Title level={3} style={{ textAlign: 'left' }}>
                        Upload File
                    </Title>

                    {/* File Upload Section */}
                    <Box
                        sx={{
                            display: 'flex',
                            flexDirection: 'column',
                            justifyContent: 'flex-start',
                            width: '100%',
                            alignItems: 'center',
                            backgroundColor: '#f5f5f5',
                            borderRadius: '10px',
                            boxShadow: '0 4px 6px rgba(0, 0, 0, 0.1)',
                        }}
                    >
                        {/*Drag and Drop area*/}
                        <Box
                            sx={{
                                textAlign: 'center',
                                padding: 8,
                            }}>
                            <Title level={3}>
                                Drag and drop files here*
                            </Title>
                            <Title level={4}>
                                or
                            </Title>
                            <input
                                type="file"
                                multiple
                                accept=".jpeg,.png,.mp4,.raw"
                                ref={fileInputRef}
                                onChange={handleFileChange}
                                style={{ display: 'none' }}
                            />
                            <Button style={{ margin: '10%' }} type="primary" htmlType="button" color="cyan" variant="solid" onClick={openFileWindow}>
                                + Add Files
                            </Button>
                            {/* File list display */}
                            <Flex wrap="wrap" style={{ marginTop: '15px', maxWidth: '100%', overflow: 'auto' }}>
                                {files.map((file, index) => (
                                    <Tag
                                        key={index}
                                        style={tagStyle}
                                    >
                                        {file.name}
                                    </Tag>
                                ))}
                            </Flex>
                        </Box>
                        {/*Constraint Info*/}
                        <Box
                            sx={{
                                textAlign: 'center',
                                padding: 1,
                            }}>
                            <Title level={4}>
                                Supports jpeg, png, mp4, raw
                            </Title>
                            <Title level={5}>
                                Maximum 100 files per upload*
                            </Title>
                        </Box>
                    </Box>
                </Box>

                {/* Right container for metadata */}
                <Box
                    sx={{
                        display: 'flex',
                        flexDirection: 'column',
                        justifyContent: 'flex-start',
                        gap: 2,
                        width: '30%',
                        minWidth: '150px',
                        overflow: 'hidden',
                        padding: 4,
                        paddingLeft: 2,
                    }}
                >
                    {/*Project Name Box*/}
                    <Box sx={metadataBoxStyle}>
                        <Title level={5}>Project name</Title>
                        <Form.Item>
                            <Input
                                value={projectName}
                                onChange={(e) => setProjectName(e.target.value)}
                            />
                        </Form.Item>
                    </Box>

                    {/*Metadata Tags Box*/}
                    <Box sx={metadataBoxStyle}>
                        <Title level={5}>Add metadata:</Title>
                        <Form.Item>
                            <Input
                                placeholder="ex. bridge"
                                value={metadataTagsInput}
                                onChange={(e) => setMetadataTagsInput(e.target.value)}
                                onPressEnter={() => handleMetadataTagAdd()}
                            />
                        </Form.Item>
                        <Flex style={{ marginBottom: "3%" }} gap="4px 0" wrap>
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
                    </Box>

                    {/*Resolution Box*/}
                    <Box sx={metadataBoxStyle}>
                        <Title level={5}>Adjust resolution:</Title>
                        <Form.Item>
                            <Select options={
                                [{ value: 'low', label: <span>Low</span> },
                                { value: 'medium', label: <span>Medium</span> },
                                { value: 'high', label: <span>High</span> }]} />
                        </Form.Item>
                    </Box>

                    {/*Date Box*/}
                    <Box sx={metadataBoxStyle}>
                        <Title level={5}>Add date:</Title>
                        <Form.Item>
                            <DatePicker
                                placeholder="Select date"
                                onChange={(date, dateString) => setSelectedDate(dateString)}
                                suffixIcon={<CalendarOutlined />}
                            />
                        </Form.Item>
                    </Box>

                    {/*Location Box*/}
                    <Box sx={metadataBoxStyle}>
                        <Title level={5}>Location:</Title>
                        <Form.Item>
                            <Input
                                value={location}
                                onChange={(e) => setLocation(e.target.value)}
                            />
                        </Form.Item>
                    </Box>

                    <Button type="primary" htmlType="button" color="cyan" variant="solid" >
                        Upload files to Project
                    </Button>
                </Box>
            </Box>
        </Box>
    )
};
