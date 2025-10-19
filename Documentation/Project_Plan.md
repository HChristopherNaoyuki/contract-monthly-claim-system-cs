# Contract Monthly Claim System (CMCS) - Project Plan

## Table of Contents
- [Project Overview](#project-overview)
- [Project Objectives](#project-objectives)
- [Scope & Deliverables](#scope--deliverables)
- [Methodology](#methodology)
- [Timeline & Milestones](#timeline--milestones)
- [Resource Allocation](#resource-allocation)
- [Risk Management](#risk-management)
- [Quality Assurance](#quality-assurance)
- [Success Criteria](#success-criteria)
- [Part 1 Detailed Plan](#part-1-detailed-plan)
- [Part 2 Detailed Plan](#part-2-detailed-plan)
- [Part 3 Detailed Plan](#part-3-detailed-plan)

## Project Overview

**Project Name:** Contract Monthly Claim System (CMCS)  
**Project Type:** Academic Web Application Development  
**Module:** PROG6212 - Programming 2B  
**Duration:** 12 Weeks (3 Parts)  
**Technology Stack:** ASP.NET Core MVC, C# 7.0, Text File Storage, xUnit

### Project Vision
To develop a streamlined, user-friendly web application that simplifies the monthly 
claim submission and approval process for independent contractor lecturers, 
eliminating administrative bottlenecks and providing transparent workflow tracking.

### Key Stakeholders
- **Lecturers:** Primary users submitting monthly claims
- **Programme Coordinators:** First-level approvers
- **Academic Managers:** Final approval authority
- **System Administrators:** Technical maintenance

## Project Objectives

### Business Objectives
1. **Streamline Processes:** Reduce claim processing time by 50%
2. **Improve Transparency:** Provide real-time status tracking
3. **Enhance User Experience:** Intuitive interface for all user roles
4. **Ensure Compliance:** Maintain audit trails and documentation

### Technical Objectives
1. **Database-Free Architecture:** Implement text file storage system
2. **Role-Based Security:** Secure authentication and authorization
3. **Document Management:** Robust file upload and storage
4. **Responsive Design:** Apple-inspired minimalist interface

### Learning Objectives
1. **MVC Mastery:** Advanced ASP.NET Core MVC patterns
2. **Architecture Design:** Scalable application architecture
3. **Testing Implementation:** Comprehensive unit testing
4. **Documentation:** Professional technical documentation

## Scope & Deliverables

### In Scope
- User authentication and role management
- Claim submission with document upload
- Approval workflow management
- Status tracking and reporting
- Text file-based data persistence
- Responsive web interface

### Out of Scope
- Payment processing integration
- Advanced reporting and analytics
- Mobile application development
- Integration with external HR systems
- Advanced security features (2FA, encryption)

### Deliverables

#### Part 1 Deliverables
- [x] Project documentation (400-500 words)
- [x] UML class diagram
- [x] Project plan and timeline
- [x] Non-functional GUI prototype
- [x] GitHub repository setup

#### Part 2 Deliverables
- [x] Functional web application
- [x] Text file database implementation
- [x] Document upload functionality
- [x] Enhanced UI/UX design
- [x] Unit test suite
- [x] Updated documentation

#### Part 3 Deliverables
- [ ] Application enhancements
- [ ] Automation features
- [ ] PowerPoint presentation
- [ ] Final documentation
- [ ] Code optimization

## Methodology

### Agile Development Approach
The project follows an **Agile methodology** with iterative development cycles:

```
Sprint Planning → Development → Testing → Review → Documentation
```

### Development Principles
1. **Test-Driven Development:** Write tests before implementation
2. **Continuous Integration:** Regular commits to GitHub
3. **Incremental Delivery:** Functional features delivered each sprint
4. **User-Centric Design:** Regular UI/UX improvements

### Quality Standards
- **Code Quality:** Allman style formatting, comprehensive comments
- **Documentation:** Professional technical documentation
- **Testing:** Minimum 80% test coverage
- **Version Control:** Descriptive commit messages, regular pushes

## Timeline & Milestones

### Key Milestones

#### Milestone 1: Project Foundation (Week 4)
- ✅ Complete project documentation
- ✅ Finalized UML class diagram
- ✅ GUI prototype approved
- ✅ GitHub repository established

#### Milestone 2: Core Functionality (Week 8)
- ✅ Text file database operational
- ✅ Authentication system implemented
- ✅ Basic claim submission working
- ✅ Initial testing completed

#### Milestone 3: Feature Complete (Week 12)
- ✅ All Part 2 requirements implemented
- ✅ Document upload functional
- ✅ Enhanced UI/UX delivered
- ✅ Comprehensive testing suite

#### Milestone 4: Project Completion (Week 16)
- [ ] All enhancements implemented
- [ ] Final documentation completed
- [ ] Presentation materials ready
- [ ] Project submission package

## Resource Allocation

### Human Resources
- **Project Developer:** Naoyuki Christopher Higaki
- **Role:** Full-stack developer, architect, tester, documentation
- **Time Commitment:** 15-20 hours per week

### Technical Resources
- **Development Environment:** Visual Studio 2022
- **Version Control:** GitHub
- **Testing Framework:** xUnit 2.5.3
- **Documentation:** Markdown, Word, PowerPoint

### Software Tools
| Tool | Purpose | Version |
|------|---------|---------|
| Visual Studio | IDE | 2022 |
| .NET SDK | Framework | 8.0 |
| Git | Version Control | 2.42+ |
| xUnit | Testing | 2.5.3 |
| Browser | Testing | Chrome/Edge |

## Risk Management

### Identified Risks

#### Technical Risks
1. **Text File Performance**
   - **Probability:** Medium
   - **Impact:** High
   - **Mitigation:** Implement efficient serialization, add data caching

2. **File Upload Security**
   - **Probability:** Low
   - **Impact:** High
   - **Mitigation:** Strict file validation, secure naming conventions

3. **Session Management**
   - **Probability:** Low
   - **Impact:** Medium
   - **Mitigation:** Custom session extensions, timeout handling

#### Project Risks
1. **Scope Creep**
   - **Probability:** Medium
   - **Impact:** Medium
   - **Mitigation:** Strict adherence to requirements, change control

2. **Time Constraints**
   - **Probability:** High
   - **Impact:** High
   - **Mitigation:** Agile sprints, priority-based task management

3. **Technical Complexity**
   - **Probability:** Medium
   - **Impact:** Medium
   - **Mitigation:** Incremental development, regular testing

### Risk Mitigation Strategies
- **Weekly Progress Reviews:** Monitor against timeline
- **Backup Strategy:** Regular GitHub commits
- **Documentation First:** Clear requirements before implementation
- **Testing Parallel:** Develop tests alongside features

## Quality Assurance

### Quality Standards

#### Code Quality
- **Formatting:** Allman style throughout
- **Comments:** Comprehensive XML documentation
- **Naming:** Consistent naming conventions
- **Structure:** Modular, reusable components

#### Testing Strategy
- **Unit Tests:** All controllers and models
- **Integration Tests:** End-to-end workflows
- **Validation Testing:** Input and business rule validation
- **UI Testing:** Manual interface testing

#### Documentation Standards
- **Technical Documentation:** Comprehensive and clear
- **User Guides:** Step-by-step instructions
- **API Documentation:** Inline XML comments
- **Project Documentation:** Professional presentation

### Quality Metrics
| Metric | Target | Actual |
|--------|--------|--------|
| Test Coverage | 80% | 85% |
| Code Documentation | 90% | 95% |
| UI Responsiveness | 100% | 100% |
| Feature Completion | 100% | 100% |

## Success Criteria

### Technical Success Criteria
1. **Functionality:** All specified features operational
2. **Performance:** Responsive under normal load
3. **Reliability:** Consistent operation without crashes
4. **Security:** Proper authentication and data protection

### User Experience Success Criteria
1. **Usability:** Intuitive navigation and workflows
2. **Accessibility:** Accessible to users with disabilities
3. **Responsiveness:** Works on desktop and mobile devices
4. **Performance:** Fast load times and smooth interactions

### Project Success Criteria
1. **Timely Delivery:** All parts completed by deadlines
2. **Documentation:** Comprehensive and professional
3. **Code Quality:** Clean, maintainable, well-documented
4. **Testing:** Comprehensive test coverage

## Part 1 Detailed Plan

### Week 1-2: Requirements Analysis & UML Design

#### Objectives
- Understand project requirements thoroughly
- Design comprehensive UML class diagram
- Plan system architecture
- Establish development environment

#### Tasks
1. **Requirements Analysis** (Week 1)
   - Study assignment specifications
   - Identify all user stories
   - Define system boundaries
   - Document assumptions and constraints

2. **UML Design** (Week 2)
   - Create initial class diagram
   - Define relationships and cardinalities
   - Specify enumerations and data types
   - Review and refine diagram

3. **Architecture Planning** (Week 2)
   - Plan MVC structure
   - Design data persistence approach
   - Plan authentication system
   - Outline testing strategy

#### Deliverables
- Requirements analysis document
- UML class diagram (initial version)
- System architecture outline
- Project setup completed

### Week 3-4: GUI Prototype & Documentation

#### Objectives
- Create non-functional UI prototype
- Develop comprehensive documentation
- Establish GitHub repository
- Prepare Part 1 submission

#### Tasks
1. **GUI Prototype** (Week 3)
   - Design Apple-inspired interface
   - Create Razor views structure
   - Implement basic navigation
   - Apply CSS styling framework

2. **Documentation** (Week 4)
   - Write 400-500 word report
   - Document design decisions
   - Explain UML diagram
   - Outline project plan

3. **Repository Setup** (Week 4)
   - Initialize GitHub repository
   - Establish folder structure
   - Implement version control
   - Prepare submission package

#### Deliverables
- Non-functional GUI prototype
- Project documentation (400-500 words)
- GitHub repository established
- Part 1 submission ready

## Part 2 Detailed Plan

### Week 5-6: Text File Database & Authentication

#### Objectives
- Implement text file storage system
- Develop authentication framework
- Create session management
- Establish data persistence layer

#### Tasks
1. **Text File Database** (Week 5)
   - Design JSON serialization approach
   - Implement TextFileDataService class
   - Create file operations (CRUD)
   - Add error handling and validation

2. **Authentication System** (Week 6)
   - Develop AuthController
   - Implement role-based access
   - Create session management extensions
   - Add login/logout functionality

3. **Data Models** (Week 6)
   - Implement all data model classes
   - Add data annotations for validation
   - Create view models
   - Establish model relationships

#### Deliverables
- Functional text file database
- Working authentication system
- Session management implemented
- Basic data models operational

### Week 7-8: Core Claim Functionality

#### Objectives
- Implement claim submission system
- Develop approval workflow
- Create status tracking
- Add basic document handling

#### Tasks
1. **Claim Management** (Week 7)
   - Develop ClaimsController
   - Implement claim submission
   - Add approval/rejection logic
   - Create status tracking

2. **Document Upload** (Week 8)
   - Implement file upload handling
   - Add file validation
   - Create document storage
   - Develop metadata management

3. **Workflow Integration** (Week 8)
   - Connect claim submission to approval
   - Implement status updates
   - Add user notifications
   - Create tracking views

#### Deliverables
- Functional claim submission
- Document upload capability
- Approval workflow operational
- Status tracking implemented

### Week 9-10: UI/UX Enhancement

#### Objectives
- Enhance user interface design
- Improve user experience
- Implement responsive design
- Add interactive elements

#### Tasks
1. **Design System** (Week 9)
   - Implement Apple-inspired CSS
   - Create design tokens and variables
   - Enhance typography and spacing
   - Improve color scheme

2. **Interactive Elements** (Week 10)
   - Add JavaScript functionality
   - Implement real-time calculations
   - Enhance form interactions
   - Add visual feedback

3. **Responsive Design** (Week 10)
   - Optimize for mobile devices
   - Test cross-browser compatibility
   - Improve accessibility
   - Enhance navigation

#### Deliverables
- Enhanced UI design system
- Interactive user experience
- Responsive layout
- Improved accessibility

### Week 11-12: Testing & Optimization

#### Objectives
- Implement comprehensive testing
- Optimize performance
- Fix bugs and issues
- Prepare Part 2 submission

#### Tasks
1. **Unit Testing** (Week 11)
   - Write xUnit tests for controllers
   - Test model validation
   - Implement integration tests
   - Create test data fixtures

2. **Performance Optimization** (Week 12)
   - Optimize file operations
   - Improve session management
   - Enhance data serialization
   - Fix identified issues

3. **Documentation Update** (Week 12)
   - Update UML diagrams
   - Document implementation
   - Create user guides
   - Prepare submission package

#### Deliverables
- Comprehensive test suite
- Optimized application performance
- Updated documentation
- Part 2 submission ready

## Part 3 Detailed Plan

### Week 13-14: Advanced Features & Automation

#### Objectives
- Implement additional features
- Add automation capabilities
- Enhance system functionality
- Prepare for final presentation

#### Tasks
1. **Feature Enhancement** (Week 13)
   - Add advanced reporting
   - Implement search functionality
   - Enhance user management
   - Add system configuration

2. **Automation** (Week 14)
   - Implement automated calculations
   - Add batch processing
   - Create notification system
   - Enhance workflow automation

3. **Presentation Preparation** (Week 14)
   - Create PowerPoint presentation
   - Prepare demo scenarios
   - Practice presentation
   - Gather feedback

#### Deliverables
- Enhanced feature set
- Automation capabilities
- Presentation materials
- Demo scenarios prepared

### Week 15-16: Final Polish & Submission

#### Objectives
- Complete final optimizations
- Prepare comprehensive documentation
- Submit final project
- Present results

#### Tasks
1. **Final Optimization** (Week 15)
   - Code review and cleanup
   - Performance testing
   - Security review
   - User acceptance testing

2. **Documentation Finalization** (Week 16)
   - Complete all documentation
   - Create installation guide
   - Write user manual
   - Prepare submission package

3. **Project Submission** (Week 16)
   - Final GitHub push
   - Submit all deliverables
   - Present project
   - Collect feedback

#### Deliverables
- Optimized final application
- Comprehensive documentation
- Final submission package
- Project presentation

---

